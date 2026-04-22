# MeteorSkill

**파일**: `Rock Spirit Idle/Assets/Scripts/Skills/MeteorSkill.cs`  
**타입**: `class MeteorSkill : SkillBase`

---

## 개요

`MeteorSkill`은 대상 적의 위치에 `spawnPos` 오프셋을 더한 지점에 메테오 오브젝트를 생성한 후, 1.82초 대기 후 `explosionRadius` 범위 내의 모든 적에게 `damageMultiplier` 배율의 데미지를 적용한다.

---

## 필드

| 필드 | 타입 | 기본값 | 설명 |
|------|------|--------|------|
| `meteorPrefab` | `GameObject` | Inspector 할당 | 메테오 시각 오브젝트 프리팹 |
| `spawnPos` | `Vector3` | `Vector3.up * 1f` | 대상 위치에 더해지는 생성 오프셋 |
| `explosionRadius` | `float` | `2f` | `Physics2D.OverlapCircleAll` 탐지 반경(월드 단위) |
| `damageMultiplier` | `float` | `12f` | `GetCurrentPower()` 대비 데미지 배율 (1200%) |

---

## ExecuteSkill

```csharp
protected override IEnumerator ExecuteSkill(Enemy target)
{
    Vector3 spawnPosition = target.transform.position + spawnPos;
    GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);

    yield return new WaitForSeconds(1.82f);

    Explode(meteor.transform.position);
    Destroy(meteor);
}
```

- `spawnPosition = target.transform.position + spawnPos` — 적 위치에서 `(0, 1, 0)` 위에 메테오 생성
- 1.82초 대기 후 `Explode`를 메테오의 현재 위치에서 호출
- `Explode` 이후 메테오 오브젝트를 즉시 파괴

---

## Explode — 범위 데미지

```csharp
private void Explode(Vector3 explosionPosition)
{
    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius);
    float damage = GameManager.Instance.player.GetCurrentPower() * damageMultiplier;

    foreach (Collider2D hit in hitEnemies)
    {
        if (hit.TryGetComponent<Enemy>(out Enemy enemy))
        {
            if (GameManager.Instance.player.CriticalHit())
            {
                damage *= GameManager.Instance.player.criticalHit;
            }
            enemy.TakeDamage(damage);
        }
    }
}
```

**데미지 계산 공식**:

```
damage = player.GetCurrentPower() * damageMultiplier(12f)
// 치명타 발생 시 (적마다 독립 판정):
damage *= player.criticalHit
```

`Physics2D.OverlapCircleAll`로 `explosionRadius(2f)` 반경 내 모든 Collider2D를 탐지한 뒤, `Enemy` 컴포넌트를 가진 오브젝트에만 데미지를 적용한다. 치명타 판정은 루프 내 각 적마다 독립적으로 수행된다.

---

## 실행 흐름

```mermaid
flowchart TD
    A["ExecuteSkill(target) 호출"] --> B["spawnPosition = target.position + spawnPos"]
    B --> C["Instantiate(meteorPrefab, spawnPosition)"]
    C --> D["WaitForSeconds(1.82f)"]
    D --> E["Explode(meteor.transform.position)"]
    E --> F["Physics2D.OverlapCircleAll(pos, explosionRadius)"]
    F --> G["GetCurrentPower() * damageMultiplier"]
    G --> H["foreach hit in hitEnemies"]
    H --> I{hit에 Enemy 컴포넌트?}
    I -- false --> J["다음 hit"]
    I -- true --> K{CriticalHit()?}
    K -- true --> L["damage *= criticalHit"]
    K -- false --> M["damage 그대로"]
    L --> N["enemy.TakeDamage(damage)"]
    M --> N
    N --> J
    J --> O{더 남은 hit?}
    O -- true --> I
    O -- false --> P["Destroy(meteor)"]
```
