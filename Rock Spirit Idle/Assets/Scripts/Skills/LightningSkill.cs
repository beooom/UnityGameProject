using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LightningSkill : MonoBehaviour
{
    public GameObject lightningEffectPrefab; // 벼락 이펙트 프리팹
    public float lightningCooldown = 5f; // 벼락 쿨타임
    public int maxTargets = 8; // 최대 타겟 수
    public float damageMultiplier = 5f; // 플레이어 공격력의 500% 데미지
    public float intervalBetweenStrikes = 0.2f; // 각 타격 간격

    public GameObject player;

    public Image cooldownImage; // 쿨타임을 표시할 이미지
    public Text cooldownText; // 쿨타임을 표시할 텍스트

    private bool isCooldown = false; // 스킬이 쿨타임 상태인지 확인

    private void Start()
    {
        // 쿨타임 이미지 초기화
        cooldownImage.fillAmount = 0f;
    }

    private void Update()
    {
        if (!isCooldown)
        {
            // 가까운 적 탐색 후 타겟이 있을 경우 스킬 실행
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
        StartCoroutine(CooldownRoutine()); // 쿨타임 표시 코루틴 시작
        foreach (Enemy target in targets)
        {
            if (target != null)
            {
                // 벼락 이펙트를 타겟 위치에 생성
                Vector3 strikePosition = target.transform.position;
                GameObject lightningEffect = Instantiate(lightningEffectPrefab, strikePosition, Quaternion.identity);

                // 타겟에 피해 적용
                float damage = GameManager.Instance.player.power * damageMultiplier;
                target.TakeDamage(damage);

                // 벼락 이펙트 삭제
                Destroy(lightningEffect, 0.8f);

                // 다음 타격까지 대기
                yield return new WaitForSeconds(intervalBetweenStrikes);
            }
        }
        //yield return new WaitForSeconds(lightningCooldown); // 쿨타임 대기
    }
    private IEnumerator CooldownRoutine()
    {
        float elapsed = 0f;
        cooldownImage.fillAmount = 1f; // 이미지가 완전히 채워진 상태로 시작

        while (elapsed < lightningCooldown)
        {
            elapsed += Time.deltaTime;
            cooldownImage.fillAmount = 1 - (elapsed / lightningCooldown); // 시간이 지남에 따라 이미지가 비워짐
            yield return null;
        }

        cooldownImage.fillAmount = 0f; // 쿨타임이 완료되면 이미지 초기화
        isCooldown = false;
    }

    private List<Enemy> FindClosestEnemies()
    {
        List<Enemy> sortedEnemies = new List<Enemy>(GameManager.Instance.enemies);

        // 플레이어 위치를 기준으로 적 정렬
        sortedEnemies.Sort((a, b) =>
        {
            if (a == null || b == null) return 0;
            float distanceA = Vector3.Distance(player.transform.position, a.transform.position);
            float distanceB = Vector3.Distance(player.transform.position, b.transform.position);
            return distanceA.CompareTo(distanceB);
        });

        // 가장 가까운 적 최대 8명 선택
        return sortedEnemies.GetRange(0, Mathf.Min(maxTargets, sortedEnemies.Count));
    }
}
