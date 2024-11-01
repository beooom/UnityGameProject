using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VoidSkill : MonoBehaviour
{
    public GameObject voidPrefab; //���̵� ������
    public float voidCooldown = 5f; //���̵� ��Ÿ��

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
                StartCoroutine(VoidSkillCo(target));
            }
        }
    }

    private IEnumerator VoidSkillCo(Enemy target)
    {
        isCooldown = true;
        StartCoroutine(CooldownRoutine()); // ��Ÿ�� ǥ�� �ڷ�ƾ ����

        Vector3 spawnPosition = GameManager.Instance.player.transform.position;
        GameObject Void = Instantiate(voidPrefab, spawnPosition, Quaternion.identity);
        yield return new WaitForSeconds(voidCooldown); // ��Ÿ�� ���
    }

    private IEnumerator CooldownRoutine()
    {
        float elapsed = 0f;
        cooldownImage.fillAmount = 1f; // �̹����� ������ ä���� ���·� ����

        while (elapsed < voidCooldown)
        {
            elapsed += Time.deltaTime;
            cooldownImage.fillAmount = 1 - (elapsed / voidCooldown); // �ð��� ������ ���� �̹����� �����
            yield return null;
        }

        cooldownImage.fillAmount = 0f; // ��Ÿ���� �Ϸ�Ǹ� �̹��� �ʱ�ȭ
        isCooldown = false;
    }

    private Enemy FindClosestEnemy()
    {
        Enemy closestEnemy = null;

        foreach (Enemy enemy in GameManager.Instance.enemies)
        {
            closestEnemy = enemy;
        }

        return closestEnemy;
    }
}
