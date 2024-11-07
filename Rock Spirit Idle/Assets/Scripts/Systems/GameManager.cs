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
    internal List<Enemy> enemies = new List<Enemy>(); //���� �����ϴ� ��ü �� List
    public List<GameObject> starlightProjectiles = new List<GameObject>();
    internal EnemySpawner spawner;
    internal Player player; //���� �����ϴ� �÷��̾� ��ü
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
            starlightProjectiles.Remove(projectile); // ����Ʈ���� ����ü ����
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
