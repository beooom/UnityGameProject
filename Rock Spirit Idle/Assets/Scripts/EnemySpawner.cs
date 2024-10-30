using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; //�� ������

    [Tooltip("�ѹ��� ������ ���� �� �ϴ� 10����")]
    public int monsterCount = 10;
    public float spawnInterval = 5f; //���� ����
    public Vector2 spawnAreaX; // x�� ���� ����
    public Vector2 spawnAreaY = new Vector2(-3.2f, -2.8f); // y�� ���� ����

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
            //���� ���� ��ġ ���� (x�� ȭ�� ������ �� ������, y�� ������ ����)
            Vector3 spawnPosition = new Vector3(Random.Range(spawnAreaX.x, spawnAreaX.y),
                                                Random.Range(spawnAreaY.x, spawnAreaY.y),
                                                0);
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }
}