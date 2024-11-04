using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class StarLightSkill : MonoBehaviour
{
    public GameObject starlightPrefab; // ��Ÿ����Ʈ ����ü ������
    public int maxStarlights = 10; // �ִ� ��Ÿ����Ʈ ��
    public float starlightCooldown = 7f; // ��Ÿ��
    public float spawnHeight = 2f; // �÷��̾� �� 3f ��ġ
    public float spawnRange = 0.5f; // 1f�� ���� ������ ���� ��ȯ
    public float fireInterval = 0.5f; // ����ü �߻� ����
    public float projectileSpeed = 5f; // ����ü �ӵ�

    public GameObject player;

    public Image cooldownImage; // ��Ÿ���� ǥ���� �̹���
    public Text cooldownText; // ��Ÿ���� ǥ���� �ؽ�Ʈ

    private List<GameObject> starlightProjectiles = new List<GameObject>(); // ������ ��Ÿ����Ʈ ����ü ����Ʈ
    private bool isCooldown = false; // ��Ÿ�� ���� Ȯ�ο�


    private void Start()
    {
        cooldownImage.fillAmount = 0f;
    }
    private void Update()
    {
        // ��ų ��Ÿ���� �ƴ� �� �� Ž�� �� ��ų �ߵ�
        if (!isCooldown)
        {
            Enemy enemy = FindClosestEnemy();
            if (enemy != null && GameManager.Instance.range.canUseSkill)
            {
                // ��Ÿ�� ���� �� ��ų �ߵ�
                isCooldown = true;
                StartCoroutine(CooldownRoutine());
                StarlightSkillRoutine(enemy);
            }
        }
    }

    private void StarlightSkillRoutine(Enemy enemy)
    {
        for (int i = 0; i < maxStarlights; i++)
        {
            Vector2 randCircle = Random.insideUnitCircle * spawnRange;
            Vector2 spawnPosition =
                new Vector2(player.transform.position.x, player.transform.position.y + spawnHeight) + randCircle;
            GameObject starlight = Instantiate(starlightPrefab, spawnPosition, Quaternion.identity);
            starlightProjectiles.Add(starlight);
        }

        // ����ü �߻� ����
        StartCoroutine(FireStarlight());
    }

    private IEnumerator FireStarlight()
    {
        while (starlightProjectiles.Count > 0)
        {
            Enemy closestEnemy = FindClosestEnemy();
            if (closestEnemy != null && GameManager.Instance.range.canUseSkill)
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
            starlight.transform.position = 
                Vector2.MoveTowards(starlight.transform.position, targetPosition, projectileSpeed * Time.deltaTime);
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
            float distance = 
                Vector3.Distance(player.transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
}
