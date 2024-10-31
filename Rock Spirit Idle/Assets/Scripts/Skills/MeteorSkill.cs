using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MeteorSkill : MonoBehaviour
{
    public GameObject meteorPrefab; // ���׿� ������
    public float meteorCooldown = 3f; // ���׿� ��Ÿ��
    public Vector3 spawnPos = Vector3.up * 1f;
    public float explosionRadius = 1f; // ���� �ݰ�
    public float damageMultiplier = 12f; // �÷��̾� ���ݷ��� 1200%

    public Image cooldownImage; // ��Ÿ���� ǥ���� �̹���
    public Text cooldownText; // ��Ÿ���� ǥ���� �ؽ�Ʈ

    private bool isCooldown = false; // ��ų�� ��Ÿ�� �������� Ȯ��

    private void Awake()
    {
        cooldownImage.fillAmount = 0f; // �ʱ�ȭ - ��Ÿ���� ���� �� �̹��� �����
    }

    private void Update()
    {
        if (!isCooldown)
        {
            Enemy target = FindClosestEnemy();
            if (target != null)
            {
                StartCoroutine(MeteorSkillRoutine(target));
            }
        }
    }

    private IEnumerator MeteorSkillRoutine(Enemy target)
    {
        isCooldown = true;
        StartCoroutine(CooldownRoutine()); // ��Ÿ�� ǥ�� �ڷ�ƾ ����
        Vector3 spawnPosition = target.transform.position + spawnPos;
        GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);

        // ���� �ð� �� ���� ó�� (�ִϸ��̼� �ð��� ��ġ�ϵ��� ����)
        yield return new WaitForSeconds(1.85f); // �ִϸ��̼� ���� �ð��� ���� ����
        Explode(meteor.transform.position);
        Destroy(meteor);

        yield return new WaitForSeconds(meteorCooldown); // ��Ÿ�� ���
    }

    private IEnumerator CooldownRoutine()
    {
        float elapsed = 0f;
        cooldownImage.fillAmount = 1f; // �̹����� ������ ä���� ���·� ����

        while (elapsed < meteorCooldown)
        {
            elapsed += Time.deltaTime;
            cooldownImage.fillAmount = 1 - (elapsed / meteorCooldown); // �ð��� ������ ���� �̹����� �����
            yield return null;
        }

        cooldownImage.fillAmount = 0f; // ��Ÿ���� �Ϸ�Ǹ� �̹��� �ʱ�ȭ
        isCooldown = false;
    }

    private void Explode(Vector3 explosionPosition)
    {
        // ���� ���� �� ��� ������ ���� ����
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius);
        float damage = GameManager.Instance.player.power * damageMultiplier;

        foreach (Collider2D hit in hitEnemies)
        {
            if (hit.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
            }
        }
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
