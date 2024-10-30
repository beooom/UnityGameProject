using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarLightProjectile : MonoBehaviour
{
    public GameObject starlightPrefab; // ��Ÿ����Ʈ ����ü ������
    public int maxStarlights = 10; // �ִ� ��Ÿ����Ʈ ��
    public float starlightCooldown = 5f; // ��Ÿ��
    public float spawnHeight = 3f; // �÷��̾� �� 3f ��ġ
    public float spawnRange = 1f; // 1f�� ���� ������ ���� ��ȯ
    public float fireInterval = 0.5f; // ����ü �߻� ����
    public float projectileSpeed = 5f; // ����ü �ӵ�

    private Transform playerTransform;
    private List<GameObject> starlightProjectiles = new List<GameObject>(); // ������ ��Ÿ����Ʈ ����ü ����Ʈ
    private bool isCooldown = false; // ��Ÿ�� ���� Ȯ�ο�


    private IEnumerator Start()
    {
        yield return null;
        playerTransform = GameManager.Instance.player.transform;
        StartCoroutine(StarlightSkillRoutine());
    }

    private IEnumerator StarlightSkillRoutine()
    {
        while (true)
        {
            if (!isCooldown && starlightProjectiles.Count == 0) // ��Ÿ���� ������ ��� ����ü�� �����Ǿ��� ��
            {
                yield return new WaitForSeconds(starlightCooldown); // ��Ÿ�� ���
                isCooldown = true;

                // ���ο� Starlight ����
                for (int i = 0; i < maxStarlights; i++)
                {
                    Vector2 randCircle = Random.insideUnitCircle;
                    Vector2 spawnPosition = 
                        new Vector2(playerTransform.position.x, playerTransform.position.y + spawnHeight) + randCircle;
                    GameObject starlight = Instantiate(starlightPrefab, spawnPosition, Quaternion.identity);
                    starlightProjectiles.Add(starlight);
                }
            }

            // ����ü�� ���� ������ �߻�
            if (starlightProjectiles.Count > 0)
            {
                yield return StartCoroutine(FireStarlight());
            }

            yield return null;
        }
    }

    private IEnumerator FireStarlight()
    {
        while (starlightProjectiles.Count > 0)
        {
            Enemy closestEnemy = FindClosestEnemy();
            if (closestEnemy != null)
            {
                // ù ��° Starlight�� �߻��ϰ� ����Ʈ���� ����
                GameObject starlight = starlightProjectiles[0];
                starlightProjectiles.RemoveAt(0);

                StartCoroutine(MoveProjectile(starlight, closestEnemy.transform.position));

                yield return new WaitForSeconds(fireInterval); // �߻� ����
            }
            else
            {
                yield return null; // ���� ������ ���
            }
        }

        // ��� ����ü �߻� �Ϸ� �� ��Ÿ�� �ʱ�ȭ
        isCooldown = false;
    }

    private IEnumerator MoveProjectile(GameObject starlight, Vector2 targetPosition)
    {
        while (starlight != null && (Vector2)starlight.transform.position != targetPosition)
        {
            starlight.transform.position = Vector2.MoveTowards(starlight.transform.position, targetPosition, projectileSpeed * Time.deltaTime);
            yield return null;
        }

        if (starlight != null)
        {
            Destroy(starlight); // ��ǥ ���� ���� �� �ı�
        }
    }

    private Enemy FindClosestEnemy()
    {
        Enemy closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (Enemy enemy in GameManager.Instance.enemies)
        {
            float distance = Vector3.Distance(playerTransform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
}
