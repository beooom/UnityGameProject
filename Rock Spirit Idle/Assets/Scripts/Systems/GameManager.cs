using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
    internal List<Enemy> enemies = new List<Enemy>(); //���� �����ϴ� ��ü �� List
    internal EnemySpawner spawner;
    internal Player player; //���� �����ϴ� �÷��̾� ��ü
    internal PlayerSkillRange range;

    internal List<StarLightSkill> starLights = new List<StarLightSkill>(); // Starlight ����Ʈ
    protected override void Awake()
    {
        base.Awake();
    }
}
