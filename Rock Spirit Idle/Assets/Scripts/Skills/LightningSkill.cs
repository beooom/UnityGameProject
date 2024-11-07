using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LightningSkill : SkillBase
{
    public GameObject lightningEffectPrefab; // ���� ����Ʈ ������
    public int maxTargets = 8; // �ִ� Ÿ�� ��
    public float damageMultiplier = 5f; // �÷��̾� ���ݷ��� 500% ������
    public float intervalBetweenStrikes = 0.2f; // �� Ÿ�� ����


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
