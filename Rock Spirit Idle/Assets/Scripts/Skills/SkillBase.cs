using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SkillBase : MonoBehaviour
{
    public float cooldown; // �� ��ų�� ��Ÿ��
    public Image cooldownImage;
    public Text cooldownText;

    public bool isCooldown = false; // ��Ÿ�� ���� Ȯ�ο�
    protected GameObject player;

    protected virtual void Awake()
    {
        cooldownImage.fillAmount = 0f;
        cooldownText.text = "";
    }
    protected virtual void Start()
    {
        player = GameManager.Instance.player.gameObject;
    }
    protected virtual void Update()
    {
        if (!isCooldown)
        {
            Enemy target = FindClosestEnemy();
            if (target != null && GameManager.Instance.range.canUseSkill)
            {
                isCooldown = true;
                StartCoroutine(CooldownRoutine());
                StartCoroutine(ExecuteSkill(target));  // �� ��ų ���� ���� ����
            }
        }
    }
    public void TriggerSkill(Enemy target)
    {
        if (!isCooldown)
        {
            isCooldown = true;
            StartCoroutine(CooldownRoutine());
            StartCoroutine(ExecuteSkill(target));  // �� ��ų ���� ���� ����
        }
    }

    protected virtual IEnumerator CooldownRoutine()
    {
        float elapsed = 0f;
        cooldownImage.fillAmount = 1f;

        while (elapsed < cooldown)
        {
            elapsed += Time.deltaTime;
            cooldownImage.fillAmount = 1 - (elapsed / cooldown);
            cooldownText.text = Mathf.Ceil(cooldown - elapsed).ToString();
            yield return null;
        }

        cooldownImage.fillAmount = 0f;
        cooldownText.text = "";
        isCooldown = false;
    }

    // ���� ����� ���� ã�� �޼���
    protected Enemy FindClosestEnemy()
    {
        Enemy closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (Enemy enemy in GameManager.Instance.enemies)
        {
            float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    // �� ��ų���� ������ ���� ����� ����
    protected abstract IEnumerator ExecuteSkill(Enemy target);
}