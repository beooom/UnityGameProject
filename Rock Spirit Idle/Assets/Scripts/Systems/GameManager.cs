using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Playing,
    Paused,
    GameOver
}

public class GameManager : SingletonManager<GameManager>
{
    internal List<Enemy> enemies = new List<Enemy>(); //씬에 존재하는 전체 적 List
    public List<GameObject> starlightProjectiles = new List<GameObject>();
    internal EnemySpawner spawner;
    internal Player player; //씬에 존재하는 플레이어 객체
    internal PlayerSkillRange range;

    public GameState currentState;
    protected override void Awake()
    {
        base.Awake();
        currentState = GameState.Playing;
    }
    public void RemoveProjectile(GameObject projectile)
    {
        if (starlightProjectiles.Contains(projectile))
        {
            starlightProjectiles.Remove(projectile); // 리스트에서 투사체 제거
        }
    }
    public void Restart()
    {
        currentState = GameState.Playing;

        spawner.monsterLevel = 0;

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            GameManager.Instance.enemies[i].Die();
        }

        foreach (GameObject star in starlightProjectiles)
        {
            Destroy(star);
        }
        starlightProjectiles.Clear();
    }
}
