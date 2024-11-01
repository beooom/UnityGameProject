using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; //적 프리팹
    private GameObject mon; 

    [Tooltip("한번에 스폰될 적의 수 일단 10마리")]
    public int monsterCount = 10;
    public int monsterLevel = 0;
    public float spawnInterval = 5f; //생성 간격
    public Vector2 spawnAreaX; // x축 생성 범위
    public Vector2 spawnAreaY = new Vector2(0.4f, 0.7f); // y축 생성 범위

    private void Awake()
    {
        spawnAreaX = new Vector2(Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0, 0)).x,
    Camera.main.ViewportToWorldPoint(new Vector3(1.6f, 0, 0)).x);
    }
    private void Start()
    {
        StartCoroutine(SpawnCoroutine());
    }

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            if (GameManager.Instance.enemies.Count == 0)
            {
                yield return new WaitForSeconds(spawnInterval);
                Spawn();
            }
            yield return null;
        }
    }
    private void Spawn()
    {
        for (int i = 0; i < monsterCount; i++)
        {
            //몬스터 생성 위치 설정 (x는 화면 오른쪽 밖 무작위, y는 무작위 높이)
            Vector3 spawnPosition = new Vector3(Random.Range(spawnAreaX.x, spawnAreaX.y),
                                                Random.Range(spawnAreaY.x, spawnAreaY.y),
                                                0);
            mon = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            mon.GetComponent<Enemy>().hp = 1 + (monsterLevel * 0.1f);
            mon.GetComponent<Enemy>().power = 1 + (monsterLevel * 1f);
        }
        monsterLevel++;
    }
}