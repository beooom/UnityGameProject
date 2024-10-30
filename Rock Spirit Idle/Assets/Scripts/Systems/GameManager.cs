using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
    internal List<Enemy> enemies = new List<Enemy>(); //씬에 존재하는 전체 적 List
    internal Player player; //씬에 존재하는 플레이어 객체

    internal List<StarLightProjectile> starLights = new List<StarLightProjectile>(); // Starlight 리스트
    protected override void Awake()
    {
        base.Awake();
    }
}
