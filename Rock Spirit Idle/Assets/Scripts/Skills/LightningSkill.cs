using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LightningSkill : SkillBase
{
    public GameObject lightningEffectPrefab; // 벼락 이펙트 프리팹
    public int maxTargets = 8; // 최대 타겟 수
    public float damageMultiplier = 5f; // 플레이어 공격력의 500% 데미지
    public float intervalBetweenStrikes = 0.2f; // 각 타격 간격


    protected override IEnumerator ExecuteSkill(Enemy target)
    {
        List<Enemy> targets = FindClosestEnemies();
        foreach (Enemy enemy in targets)
        {
            if (enemy != null)
            {
                Vector3 strikePosition = enemy.transform.position;
                GameObject lightningEffect = Instantiate(lightningEffectPrefab, strikePosition, Quaternion.identity);

                float damage = GameManager.Instance.player.GetCurrentPower() * damageMultiplier;
                if (GameManager.Instance.player.CriticalHit())
                {
                    damage *= GameManager.Instance.player.criticalHit;
                }
                enemy.TakeDamage(damage);
                Destroy(lightningEffect, 0.8f);

                yield return new WaitForSeconds(intervalBetweenStrikes);
            }
        }
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
