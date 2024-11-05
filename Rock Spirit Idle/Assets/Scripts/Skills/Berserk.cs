using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Berserk : MonoBehaviour
{
    public float berserkCooldown = 20f; // ��Ÿ��
    public float durationTime = 10f; //���ӽð�
    public float powerUP = 2f; // ���ݷ� ���� ��ġ
    public float playerPower;

    public Animator anim;

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
            playerPower = GameManager.Instance.player.power;
            Enemy target = FindClosestEnemy();
            if (target != null && GameManager.Instance.range.canUseSkill)
            {
                StartCoroutine(BerserkRoutine(target));
            }
        }
    }
    private IEnumerator BerserkRoutine(Enemy target)
    {
        isCooldown = true;
        StartCoroutine(CooldownRoutine()); // ��Ÿ�� ǥ�� �ڷ�ƾ ����

        anim.SetBool("berserking", true); // �ִϸ��̼� Ű�鼭 
        GameManager.Instance.player.berserkMultiplier = powerUP; // ���ݷ� ��
        yield return new WaitForSeconds(durationTime); // ���ӽð� ���

        anim.SetBool("berserking", false); // �ִϸ��̼� ���鼭
        GameManager.Instance.player.berserkMultiplier = 1f; //�ٽ� �������
        yield return new WaitForSeconds(berserkCooldown); // ��Ÿ�� ���
    }

    private IEnumerator CooldownRoutine()
    {
        float elapsed = 0f;
        cooldownImage.fillAmount = 1f; // �̹����� ������ ä���� ���·� ����

        while (elapsed < berserkCooldown)
        {
            elapsed += Time.deltaTime;
            cooldownImage.fillAmount = 1 - (elapsed / berserkCooldown); // �ð��� ������ ���� �̹����� �����
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
