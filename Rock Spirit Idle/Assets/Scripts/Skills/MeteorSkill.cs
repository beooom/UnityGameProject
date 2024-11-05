using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class MeteorSkill : MonoBehaviour
{
    public GameObject meteorPrefab; // 메테오 프리팹
    public float meteorCooldown = 3f; // 메테오 쿨타임
    public Vector3 spawnPos = Vector3.up * 1f;
    public float explosionRadius = 2f; // 폭발 반경
    public float damageMultiplier = 12f; // 플레이어 공격력의 1200%

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
            Enemy target = FindClosestEnemy();
            if (target != null && GameManager.Instance.range.canUseSkill)
            {
                StartCoroutine(MeteorSkillRoutine(target));
            }
        }
    }

    private IEnumerator MeteorSkillRoutine(Enemy target)
    {
        isCooldown = true;
        StartCoroutine(CooldownRoutine()); // 쿨타임 표시 코루틴 시작
        Vector3 spawnPosition = target.transform.position + spawnPos;
        GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);

        // 일정 시간 후 폭발 처리 (애니메이션 시간과 일치하도록 설정)
        yield return new WaitForSeconds(1.82f); // 애니메이션 지속 시간에 맞춰 설정
        Explode(meteor.transform.position);
        Destroy(meteor);

        yield return new WaitForSeconds(meteorCooldown); // 쿨타임 대기
    }

    private IEnumerator CooldownRoutine()
    {
        float elapsed = 0f;
        cooldownImage.fillAmount = 1f; // 이미지가 완전히 채워진 상태로 시작

        while (elapsed < meteorCooldown)
        {
            elapsed += Time.deltaTime;
            cooldownImage.fillAmount = 1 - (elapsed / meteorCooldown); // 시간이 지남에 따라 이미지가 비워짐
            yield return null;
        }

        cooldownImage.fillAmount = 0f; // 쿨타임이 완료되면 이미지 초기화
        isCooldown = false;
    }

    private void Explode(Vector3 explosionPosition)
    {
        // 폭발 범위 내 모든 적에게 피해 적용
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius);
        float damage = GameManager.Instance.player.GetCurrentPower() * damageMultiplier;

        foreach (Collider2D hit in hitEnemies)
        {
            if (hit.TryGetComponent<Enemy>(out Enemy enemy))
            {
                if (GameManager.Instance.player.CriticalHit())
                    enemy.TakeDamage(damage * player.GetComponent<Player>().criticalHit);
                else
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
            float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
}
