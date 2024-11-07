using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SkillBase : MonoBehaviour
{
    public float cooldown; // 각 스킬의 쿨타임
    public Image cooldownImage;
    public Text cooldownText;

    public bool isCooldown = false; // 쿨타임 여부 확인용
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
                StartCoroutine(ExecuteSkill(target));  // 각 스킬 고유 동작 실행
            }
        }
    }
    public void TriggerSkill(Enemy target)
    {
        if (!isCooldown)
        {
            isCooldown = true;
            StartCoroutine(CooldownRoutine());
            StartCoroutine(ExecuteSkill(target));  // 각 스킬 고유 동작 실행
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

    // 가장 가까운 적을 찾는 메서드
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

    // 각 스킬마다 고유한 실행 방식을 강제
    protected abstract IEnumerator ExecuteSkill(Enemy target);
}