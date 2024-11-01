using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; //�� ������
    private GameObject mon; 

    [Tooltip("�ѹ��� ������ ���� �� �ϴ� 10����")]
    public int monsterCount = 10;
    public int monsterLevel = 0;
    public float spawnInterval = 5f; //���� ����
    public Vector2 spawnAreaX; // x�� ���� ����
    public Vector2 spawnAreaY = new Vector2(0.4f, 0.7f); // y�� ���� ����

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
            //���� ���� ��ġ ���� (x�� ȭ�� ������ �� ������, y�� ������ ����)
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