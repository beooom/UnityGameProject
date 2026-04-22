# LightningSkill

**파일**: `Rock Spirit Idle/Assets/Scripts/Skills/LightningSkill.cs`  
**타입**: `class LightningSkill : SkillBase`

---

## 개요

`LightningSkill`은 플레이어에게 가까운 순서로 최대 `maxTargets`개의 적을 선정한 뒤, `intervalBetweenStrikes` 간격으로 번개 이펙트와 데미지를 순차적으로 적용한다.

---

## 필드

| 필드 | 타입 | 기본값 | 설명 |
|------|------|--------|------|
| `lightningEffectPrefab` | `GameObject` | Inspector 할당 | 번개 시각 이펙트 프리팹 |
| `maxTargets` | `int` | `8` | 한 번에 타격할 최대 적 수 |
| `damageMultiplier` | `float` | `5f` | `GetCurrentPower()` 대비 데미지 배율 (500%) |
| `intervalBetweenStrikes` | `float` | `0.2f` | 각 적 타격 사이의 대기 시간(초) |

---

## FindClosestEnemies — 거리 기반 정렬 후 슬라이스

```csharp
private List<Enemy> FindClosestEnemies()
{
    List<Enemy> sortedEnemies = new List<Enemy>(GameManager.Instance.enemies);

    sortedEnemies.Sort((a, b) =>
    {
        if (a == null || b == null) return 0;
        float distanceA = Vector3.Distance(player.transform.position, a.transform.position);
        float distanceB = Vector3.Distance(player.transform.position, b.transform.position);
        return distanceA.CompareTo(distanceB);
    });

    return sortedEnemies.GetRange(0, Mathf.Min(maxTargets, sortedEnemies.Count));
}
```

1. `GameManager.Instance.enemies`를 복사하여 `sortedEnemies` 생성
2. 플레이어로부터의 `Vector3.Distance` 기준 오름차순 정렬
3. `Mathf.Min(maxTargets, sortedEnemies.Count)` — 적의 수가 `maxTargets`보다 적을 경우 전체 반환

---

## ExecuteSkill — 순차 타격 루프

```csharp
protected override IEnumerator ExecuteSkill(Enemy target)
{
    List<Enemy> targets = FindClosestEnemies();
    foreach (Enemy enemy in targets)
    {
        if (enemy != null)
        {
            Vector3 strikePosition = enemy.transform.position;
            GameObject lightningEffect = Instantiate(lightningEffectPrefab, strikePosition, Quaternion.identity);

            float damage = GameManager.Instance.player.GetCurrentPower() * damageMultiplier;
            if (GameManager.Instance.player.CriticalHit())
            {
                damage *= GameManager.Instance.player.criticalHit;
            }
            enemy.TakeDamage(damage);
            Destroy(lightningEffect, 0.8f);

            yield return new WaitForSeconds(intervalBetweenStrikes);
        }
    }
}
```

**데미지 계산 공식**:

```
damage = player.GetCurrentPower() * damageMultiplier(5f)
// 치명타 발생 시:
damage *= player.criticalHit
```

각 적에 번개 이펙트를 생성하고, 데미지를 적용한 뒤 `intervalBetweenStrikes(0.2f)` 초 대기한다. 번개 이펙트는 0.8초 후 자동 파괴된다.

---

## 실행 흐름

```mermaid
flowchart TD
    A["ExecuteSkill(target) 호출"] --> B["FindClosestEnemies()"]
    B --> C["GameManager.enemies 복사"]
    C --> D["Vector3.Distance 기준 오름차순 정렬"]
    D --> E["GetRange(0, Min(maxTargets, count))"]
    E --> F["foreach enemy in targets"]
    F --> G{enemy != null?}
    G -- false --> H["다음 enemy"]
    G -- true --> I["lightningEffectPrefab Instantiate"]
    I --> J["GetCurrentPower() * damageMultiplier"]
    J --> K{CriticalHit()?}
    K -- true --> L["damage *= criticalHit"]
    K -- false --> M["damage 그대로"]
    L --> N["enemy.TakeDamage(damage)"]
    M --> N
    N --> O["Destroy(lightningEffect, 0.8f)"]
    O --> P["WaitForSeconds(intervalBetweenStrikes)"]
    P --> H
    H --> Q{더 남은 enemy?}
    Q -- true --> G
    Q -- false --> R["코루틴 종료"]
```
