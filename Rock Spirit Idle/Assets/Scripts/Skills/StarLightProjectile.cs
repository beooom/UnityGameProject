using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class StarLightProjectile : MonoBehaviour
{
    public GameObject starlightPrefab; // ��Ÿ����Ʈ ����ü ������
    public int maxStarlights = 10; // �ִ� ��Ÿ����Ʈ ��
    public float starlightCooldown = 7f; // ��Ÿ��
    public float spawnHeight = 2f; // �÷��̾� �� 3f ��ġ
    public float spawnRange = 0.5f; // 1f�� ���� ������ ���� ��ȯ
    public float fireInterval = 0.5f; // ����ü �߻� ����
    public float projectileSpeed = 5f; // ����ü �ӵ�

    private List<GameObject> starlightProjectiles = new List<GameObject>(); // ������ ��Ÿ����Ʈ ����ü ����Ʈ
    private bool isCooldown = false; // ��Ÿ�� ���� Ȯ�ο�

    public Image cooldownImage; // ��Ÿ���� ǥ���� �̹���
    public Text cooldownText; // ��Ÿ���� ǥ���� �ؽ�Ʈ

    private void Awake()
    {
        cooldownImage.fillAmount = 0f; // �ʱ�ȭ - ��Ÿ���� ���� �� �̹��� �����
    }
    private void Start()
    {
        StartCoroutine(StarlightSkillRoutine());
    }
    private IEnumerator StarlightSkillRoutine()
    {
        while (true)
        {
            Enemy enemy = FindClosestEnemy();
            if (enemy != null && !isCooldown)
            {
                // ��Ÿ�� ����
                isCooldown = true;
                StartCoroutine(CooldownRoutine()); // ��Ÿ�� ǥ�� �ڷ�ƾ ����

                for (int i = 0; i < maxStarlights; i++)
                {
                    Vector2 randCircle = Random.insideUnitCircle * spawnRange;
                    Vector2 spawnPosition =
                        new Vector2(GameManager.Instance.player.transform.position.x,
                        GameManager.Instance.player.transform.position.y + spawnHeight) + randCircle;
                    GameObject starlight = Instantiate(starlightPrefab, spawnPosition, Quaternion.identity);
                    starlightProjectiles.Add(starlight);
                }

                // ����ü�� ���� ������ �߻�
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
    private IEnumerator CooldownRoutine()
    {
        float elapsed = 0f;
        cooldownImage.fillAmount = 1f; // �̹����� ������ ä���� ���·� ����

        while (elapsed < starlightCooldown)
        {
            elapsed += Time.deltaTime;
            cooldownImage.fillAmount = 1 - (elapsed / starlightCooldown); // �ð��� ������ ���� �̹����� �����
            yield return null;
        }

        cooldownImage.fillAmount = 0f; // ��Ÿ���� �Ϸ�Ǹ� �̹��� �ʱ�ȭ
        isCooldown = false;
    }

    private Enemy FindClosestEnemy()
    {
        Enemy closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (Enemy enemy in GameManager.Instance.enemies)
        {
            float distance = Vector3.Distance(GameManager.Instance.player.transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
}
