using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LightningSkill : MonoBehaviour
{
    public GameObject lightningEffectPrefab; // ���� ����Ʈ ������
    public float lightningCooldown = 5f; // ���� ��Ÿ��
    public int maxTargets = 8; // �ִ� Ÿ�� ��
    public float damageMultiplier = 5f; // �÷��̾� ���ݷ��� 500% ������
    public float intervalBetweenStrikes = 0.2f; // �� Ÿ�� ����

    public GameObject player;

    public Image cooldownImage; // ��Ÿ���� ǥ���� �̹���
    public Text cooldownText; // ��Ÿ���� ǥ���� �ؽ�Ʈ

    private bool isCooldown = false; // ��ų�� ��Ÿ�� �������� Ȯ��

    private void Start()
    {
        // ��Ÿ�� �̹��� �ʱ�ȭ
        cooldownImage.fillAmount = 0f;
    }

    private void Update()
    {
        if (!isCooldown)
        {
            // ����� �� Ž�� �� Ÿ���� ���� ��� ��ų ����
            List<Enemy> targets = FindClosestEnemies();
            if (targets.Count > 0 && GameManager.Instance.range.canUseSkill)
            {
                StartCoroutine(LightningSkillRoutine(targets));
            }
        }
    }

    private IEnumerator LightningSkillRoutine(List<Enemy> targets)
    {
        isCooldown = true;
        StartCoroutine(CooldownRoutine()); // ��Ÿ�� ǥ�� �ڷ�ƾ ����
        foreach (Enemy target in targets)
        {
            if (target != null)
            {
                // ���� ����Ʈ�� Ÿ�� ��ġ�� ����
                Vector3 strikePosition = target.transform.position;
                GameObject lightningEffect = Instantiate(lightningEffectPrefab, strikePosition, Quaternion.identity);

                // Ÿ�ٿ� ���� ����
                float damage = GameManager.Instance.player.power * damageMultiplier;
                target.TakeDamage(damage);

                // ���� ����Ʈ ����
                Destroy(lightningEffect, 0.8f);

                // ���� Ÿ�ݱ��� ���
                yield return new WaitForSeconds(intervalBetweenStrikes);
            }
        }
        //yield return new WaitForSeconds(lightningCooldown); // ��Ÿ�� ���
    }
    private IEnumerator CooldownRoutine()
    {
        float elapsed = 0f;
        cooldownImage.fillAmount = 1f; // �̹����� ������ ä���� ���·� ����

        while (elapsed < lightningCooldown)
        {
            elapsed += Time.deltaTime;
            cooldownImage.fillAmount = 1 - (elapsed / lightningCooldown); // �ð��� ������ ���� �̹����� �����
            yield return null;
        }

        cooldownImage.fillAmount = 0f; // ��Ÿ���� �Ϸ�Ǹ� �̹��� �ʱ�ȭ
        isCooldown = false;
    }

    private List<Enemy> FindClosestEnemies()
    {
        List<Enemy> sortedEnemies = new List<Enemy>(GameManager.Instance.enemies);

        // �÷��̾� ��ġ�� �������� �� ����
        sortedEnemies.Sort((a, b) =>
        {
            if (a == null || b == null) return 0;
            float distanceA = Vector3.Distance(player.transform.position, a.transform.position);
            float distanceB = Vector3.Distance(player.transform.position, b.transform.position);
            return distanceA.CompareTo(distanceB);
        });

        // ���� ����� �� �ִ� 8�� ����
        return sortedEnemies.GetRange(0, Mathf.Min(maxTargets, sortedEnemies.Count));
    }
}
