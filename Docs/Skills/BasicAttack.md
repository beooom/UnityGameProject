# BasicAttack

**파일**: `Rock Spirit Idle/Assets/Scripts/Skills/BasicAttack.cs`  
**투사체 파일**: `Rock Spirit Idle/Assets/Scripts/Skills/basicProjectile.cs`  
**타입**: `class BasicAttack : SkillBase`

---

## 개요

`SkillBase`의 `Update` 자동 발동 루프를 그대로 사용하되, `ExecuteSkill`이 무한 루프(`while (true)`)로 동작하여 공격 속도 간격마다 투사체를 발사한다. 쿨다운 UI가 없으므로 `CooldownRoutine`을 `yield break`로 오버라이드한다.

---

## 필드

| 필드 | 타입 | 설명 |
|------|------|------|
| `projectilePrefab` | `GameObject` | 발사할 투사체 프리팹 (`BasicProjectile` 컴포넌트 포함) |

---

## Awake 오버라이드

```csharp
protected override void Awake()
{
    cooldownImage = null;
    cooldownText = null;
}
```

`SkillBase.Awake`의 `cooldownImage.fillAmount` 접근을 방지하기 위해 UI 참조를 `null`로 설정한다.

---

## ExecuteSkill — 무한 공격 루프

```csharp
protected override IEnumerator ExecuteSkill(Enemy target)
{
    while (true)
    {
        target = FindClosestEnemy();
        if (target != null && GameManager.Instance.range.canUseSkill)
        {
            FireProjectile(target);
            if (DoubleShot())
            {
                yield return new WaitForSeconds(0.1f);
                FireProjectile(target);
            }
            float attackInterval = 1f / (GameManager.Instance.player.attackSpeed + GameManager.Instance.player.attackSpeedIncrease);
            yield return new WaitForSeconds(attackInterval);
        }
        else
            yield return null;
    }
}
```

**공격 속도 계산 공식**:

```
attackInterval = 1f / (player.attackSpeed + player.attackSpeedIncrease)
```

`attackSpeed`와 `attackSpeedIncrease`의 합이 초당 공격 횟수(Attacks Per Second)가 된다. `attackInterval`이 짧을수록 더 빠르게 공격한다.

루프는 매 공격마다 `FindClosestEnemy()`를 다시 호출하여 현재 가장 가까운 적을 재탐색한다.

---

## FireProjectile

```csharp
private void FireProjectile(Enemy target)
{
    if (target == null) return;

    GameObject projectile = Instantiate(projectilePrefab, player.transform.position, Quaternion.identity);
    BasicProjectile projectileScript = projectile.GetComponent<BasicProjectile>();
    projectileScript.SetTarget(target);
}
```

플레이어 위치에서 투사체를 생성하고 `BasicProjectile.SetTarget`에 적 참조를 전달한다.

---

## DoubleShot — 조건

```csharp
private bool DoubleShot()
{
    return GameManager.Instance.player.doubleShot > Random.Range(0, 100);
}
```

`player.doubleShot`이 0~99 사이의 난수보다 클 때 `true`를 반환한다. `doubleShot` 값이 클수록 이중 발사 확률이 높아진다. `doubleShot == 0`이면 이중 발사가 발생하지 않는다.

---

## CooldownRoutine 오버라이드

```csharp
protected override IEnumerator CooldownRoutine()
{
    yield break;
}
```

`BasicAttack`은 쿨다운 없이 무한 루프로 동작하므로 `CooldownRoutine`을 즉시 종료시킨다.

---

## BasicProjectile

**파일**: `Rock Spirit Idle/Assets/Scripts/Skills/basicProjectile.cs`  
**타입**: `class BasicProjectile : MonoBehaviour`

### 필드

| 필드/메서드 | 타입 | 설명 |
|------|------|------|
| `curve` | `AnimationCurve` | 포물선 높이 보정 커브 |
| `flightSpeed` | `float` | 비행 지속 시간(초). 값이 클수록 느리게 이동 |
| `hoverHeight` | `float` | 포물선의 최대 높이 |
| `isUsed` | `bool` (private) | 첫 충돌 처리 후 중복 충돌 방지 플래그 |
| `SetTarget(Enemy)` | `public void` | 타겟을 지정하고 `FireCurve()` 코루틴을 시작. 타겟이 null이면 즉시 자신을 Destroy |

### FireCurve — AnimationCurve 기반 포물선 이동

```csharp
private IEnumerator FireCurve()
{
    if (target == null)
    {
        Destroy(gameObject);
    }

    float duration = flightSpeed;
    float time = 0.0f;
    Vector3 start = transform.position;
    Vector3 end = target.transform.position;

    while (time < duration)
    {
        time += Time.deltaTime;
        float linearT = time / duration;
        float heightT = curve.Evaluate(linearT);
        float height = Mathf.Lerp(0.0f, hoverHeight, heightT);

        transform.position = Vector2.Lerp(start, end, linearT) + new Vector2(0.0f, height);
        yield return null;
    }

    Destroy(gameObject);
}
```

- `linearT = time / duration`: 0→1 선형 진행 비율
- `heightT = curve.Evaluate(linearT)`: `AnimationCurve`로 높이 보정값 샘플링
- `height = Mathf.Lerp(0f, hoverHeight, heightT)`: 실제 Y 오프셋
- 최종 위치: `Vector2.Lerp(start, end, linearT) + (0, height)`

목표 위치는 발사 시점에 고정되므로(`end = target.transform.position`), 이후 적이 이동해도 원래 위치로 날아간다. `duration` 경과 후 충돌 없이 `Destroy`된다.

### OnTriggerEnter2D — 데미지 계산

```csharp
private void OnTriggerEnter2D(Collider2D collision)
{
    if (isUsed) return;

    if (collision.TryGetComponent<Enemy>(out Enemy enemy))
    {
        isUsed = true;
        float damage = GameManager.Instance.player.GetCurrentPower();
        if (GameManager.Instance.player.CriticalHit())
        {
            damage *= GameManager.Instance.player.criticalHit;
        }
        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}
```

`isUsed` 플래그로 동일 프레임 내 다중 충돌을 방지한다. 데미지 = `GetCurrentPower()`, 치명타 발생 시 `criticalHit` 배율을 곱한다.

---

## 투사체 생명주기

```mermaid
flowchart TD
    A["BasicAttack.FireProjectile()"] --> B["Instantiate(projectilePrefab)"]
    B --> C["BasicProjectile.SetTarget(target)"]
    C --> D{target == null?}
    D -- true --> E["Destroy(gameObject)"]
    D -- false --> F["StartCoroutine(FireCurve())"]
    F --> G["AnimationCurve 포물선 이동 루프"]
    G --> H{time >= duration?}
    H -- false --> G
    H -- true --> I["Destroy(gameObject)"]
    G --> J["OnTriggerEnter2D 충돌 감지"]
    J --> K{isUsed?}
    K -- true --> L["return 무시"]
    K -- false --> M["isUsed = true"]
    M --> N["GetCurrentPower()"]
    N --> O{CriticalHit()?}
    O -- true --> P["damage *= criticalHit"]
    O -- false --> Q["damage 그대로"]
    P --> R["enemy.TakeDamage(damage)"]
    Q --> R
    R --> S["Destroy(gameObject)"]
```
