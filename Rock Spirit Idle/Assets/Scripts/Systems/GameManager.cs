using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
    internal List<Enemy> enemies = new List<Enemy>(); //���� �����ϴ� ��ü �� List
    internal Player player; //���� �����ϴ� �÷��̾� ��ü

    internal List<StarLightProjectile> starLights = new List<StarLightProjectile>(); // Starlight ����Ʈ
    protected override void Awake()
    {
        base.Awake();
    }
}
