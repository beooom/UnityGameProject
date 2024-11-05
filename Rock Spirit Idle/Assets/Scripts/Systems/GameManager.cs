using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
    internal List<Enemy> enemies = new List<Enemy>(); //씬에 존재하는 전체 적 List
    internal EnemySpawner spawner;
    internal Player player; //씬에 존재하는 플레이어 객체
    internal PlayerSkillRange range;

    internal List<StarLightSkill> starLights = new List<StarLightSkill>(); // Starlight 리스트
    protected override void Awake()
    {
        base.Awake();
    }
}
