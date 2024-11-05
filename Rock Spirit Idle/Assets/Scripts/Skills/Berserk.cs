using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Berserk : MonoBehaviour
{
    public float berserkCooldown = 20f; // 쿨타임
    public float durationTime = 10f; //지속시간
    public float powerUP = 2f; // 공격력 업할 수치
    public float playerPower;

    public Animator anim;

    public Image cooldownImage; // 쿨타임을 표시할 이미지
    public Text cooldownText; // 쿨타임을 표시할 텍스트

    private bool isCooldown = false; // 스킬이 쿨타임 상태인지 확인

    private void Awake()
    {
        cooldownImage.fillAmount = 0f; // 초기화 - 쿨타임이 없을 때 이미지 비워둠
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
        StartCoroutine(CooldownRoutine()); // 쿨타임 표시 코루틴 시작

        anim.SetBool("berserking", true); // 애니메이션 키면서 
        GameManager.Instance.player.berserkMultiplier = powerUP; // 공격력 업
        yield return new WaitForSeconds(durationTime); // 지속시간 대기

        anim.SetBool("berserking", false); // 애니메이션 끄면서
        GameManager.Instance.player.berserkMultiplier = 1f; //다시 원래대로
        yield return new WaitForSeconds(berserkCooldown); // 쿨타임 대기
    }

    private IEnumerator CooldownRoutine()
    {
        float elapsed = 0f;
        cooldownImage.fillAmount = 1f; // 이미지가 완전히 채워진 상태로 시작

        while (elapsed < berserkCooldown)
        {
            elapsed += Time.deltaTime;
            cooldownImage.fillAmount = 1 - (elapsed / berserkCooldown); // 시간이 지남에 따라 이미지가 비워짐
            yield return null;
        }

        cooldownImage.fillAmount = 0f; // 쿨타임이 완료되면 이미지 초기화
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
