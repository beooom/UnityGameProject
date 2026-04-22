# Character

## Purpose

Character 도메인은 플레이어(`Player`)와 적(`Enemy`)의 스탯, 이동, 공격, 사망 처리를 담당하며, `EnemySpawner`를 통해 웨이브 단위로 적을 생성하고 `PlayerSkillRange`를 통해 플레이어의 공격 가능 여부를 판정한다.

## Architecture

```mermaid
classDiagram
    class Player {
        +float power
        +float hp
        +float maxHp
        +float criticalRate
        +float criticalHit
        +float attackSpeed
        +float berserkMultiplier
        +BasicAttack basicAttack
        +GetCurrentPower() float
        +CriticalHit() bool
        +TakeDamage(float)
        +Die() IEnumerator
    }
    class Enemy {
        +float hp
        +float maxHp
        +float power
        +float attackSpeed
        +TakeDamage(float)
        +Die()
    }
    class EnemySpawner {
        +GameObject enemyPrefab
        +int monsterCount
        +int monsterLevel
        +float spawnInterval
        +Vector2 spawnAreaY
        -Spawn()
    }
    class PlayerSkillRange {
        +bool canUseSkill
    }
    class BasicAttack {
        +GameObject projectilePrefab
        +TriggerSkill(Enemy)
    }
    class GameManager {
        +List~Enemy~ enemies
        +Player player
        +EnemySpawner spawner
        +PlayerSkillRange range
    }

    Player --> BasicAttack : GetComponentInChildren
    Player --> GameManager : registers self
    Player --> PlayerSkillRange : reads canUseSkill
    Enemy --> GameManager : registers and removes self
    EnemySpawner --> Enemy : Instantiate
    EnemySpawner --> GameManager : registers self
    PlayerSkillRange --> GameManager : registers self
    GameManager o-- Enemy : enemies list
    GameManager o-- Player : player ref
    GameManager o-- EnemySpawner : spawner ref
    GameManager o-- PlayerSkillRange : range ref
```

## Key Components

| Class | File | Role |
|---|---|---|
| `Player` | `Rock Spirit Idle/Assets/Scripts/Character/Player.cs` | 플레이어 스탯 보유, 매 Update에서 가장 가까운 적 탐색 후 `BasicAttack` 호출, HP 회복 코루틴 실행, 사망 시 게임 재시작 처리 |
| `Enemy` | `Rock Spirit Idle/Assets/Scripts/Character/Enemy.cs` | 적 스탯 보유, 플레이어 방향으로 이동, 플레이어 충돌 시 공격 코루틴 시작, 사망 시 골드 지급 후 자기 자신 제거 |
| `EnemySpawner` | `Rock Spirit Idle/Assets/Scripts/Character/EnemySpawner.cs` | 적 리스트가 빌 때마다 웨이브를 스폰하고 `monsterLevel`에 따라 HP/공격력을 스케일링 |
| `PlayerSkillRange` | `Rock Spirit Idle/Assets/Scripts/Character/PlayerSkillRange.cs` | 2D 트리거 영역 안에 적이 있는지 감지하여 `canUseSkill` 플래그 제공 |

## Dependencies

- **Depends on:**
  - `GameManager` (`Rock Spirit Idle/Assets/Scripts/Systems/GameManager.cs`) — `enemies`, `player`, `spawner`, `range`, `currentState`, `Restart()` 접근
  - `DataManager` — `Enemy.CheckEnemyDeath`에서 `DataManager.Instance.totalGold` 증가
  - `BasicAttack` (`Rock Spirit Idle/Assets/Scripts/Skills/BasicAttack.cs`) — `Player`가 `GetComponentInChildren`으로 획득하여 `TriggerSkill` 호출
  - `SingletonManager<T>` — `GameManager`의 기반 클래스

- **Depended by:**
  - `Skills` 도메인 — `BasicAttack`, `SkillBase` 등이 `Player`, `Enemy`, `GameManager.Instance.player/range`를 참조
  - `UI` 도메인 — HP 바(`Image hpBar`, `Text curHp`)를 Player/Enemy가 직접 보유

## Data Flow

```mermaid
flowchart TD
    A["EnemySpawner.Spawn()"] --> B["Enemy 인스턴스 생성\nhp = 1 + monsterLevel × 0.1\npower = 1 + monsterLevel × 1"]
    B --> C["Enemy.Start()\nGameManager.enemies.Add(this)"]
    C --> D["Enemy.Move() 코루틴\n플레이어 방향으로 이동"]
    D --> E["PlayerSkillRange.OnTriggerEnter2D\ncanUseSkill = true"]
    E --> F["Player.Update()\nFindClosestEnemy() 탐색"]
    F --> G["BasicAttack.TriggerSkill(target)"]
    G --> H["BasicProjectile 생성\n플레이어 power × berserkMultiplier"]
    H --> I["Enemy.TakeDamage(damage)\nhp -= damage"]
    I --> J{"hp <= 0?"}
    J -- No --> D
    J -- Yes --> K["CheckEnemyDeath 코루틴\nDataManager.totalGold += 10\nanim Die 트리거"]
    K --> L["Enemy.Die()\nGameManager.enemies.Remove\nDestroy(gameObject)"]
    L --> M{"enemies.Count == 0?"}
    M -- Yes --> A
    M -- No --> F

    N["Enemy.OnTriggerEnter2D Player"] --> O["Enemy.Attack() 코루틴\nplayer.TakeDamage(power)"]
    O --> P{"Player hp <= 0?"}
    P -- Yes --> Q["Player.Die()\nGameState.GameOver\nGameManager.Restart()"]
    Q --> A
```

---

## 세부 문서

| 문서 | 대상 클래스 |
|------|------------|
| `Docs/Character/Player.md` | `Player` |
| `Docs/Character/Enemy.md` | `Enemy`, `EnemySpawner` |
