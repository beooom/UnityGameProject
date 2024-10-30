using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; //적 프리팹

    [Tooltip("한번에 스폰될 적의 수 일단 10마리")]
    public int monsterCount = 10;
    public float spawnInterval = 5f; //생성 간격
    public Vector2 spawnAreaX; // x축 생성 범위
    public Vector2 spawnAreaY = new Vector2(-3.2f, -2.8f); // y축 생성 범위

    private void Start()
    {
        spawnAreaX = new Vector2(Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0, 0)).x,
            Camera.main.ViewportToWorldPoint(new Vector3(1.5f, 0, 0)).x);
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
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }
}