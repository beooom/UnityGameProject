using System.Collections;
using UnityEngine;

public class BasicAttack : SkillBase
{
    public GameObject projectilePrefab;

    protected override void Awake()
    {
        cooldownImage = null;
        cooldownText = null;
    }
    // ���� �ӵ��� ���� ���������� �����ϴ� �ڷ�ƾ
    protected override IEnumerator ExecuteSkill(Enemy target)
    {
        while (true)
        {
            target = FindClosestEnemy();
            if (target != null && GameManager.Instance.range.canUseSkill)
            {
                FireProjectile(target);
                // ���� ���� Ȯ�� �� �߰� ���� ����
                if (DoubleShot())
                {
                    yield return new WaitForSeconds(0.1f);
                    FireProjectile(target);
                }
                float attackInterval = 1f / (GameManager.Instance.player.attackSpeed + GameManager.Instance.player.attackSpeedIncrease);
                yield return new WaitForSeconds(attackInterval);
            }
            else
                yield return null;
        }
    }
    private void FireProjectile(Enemy target)
    {
        if (target == null) return;

        GameObject projectile = Instantiate(projectilePrefab, player.transform.position, Quaternion.identity);
        BasicProjectile projectileScript = projectile.GetComponent<BasicProjectile>();
        projectileScript.SetTarget(target);
    }

    private bool DoubleShot()
    {
        return GameManager.Instance.player.doubleShot > Random.Range(0, 100);
    }
    protected override IEnumerator CooldownRoutine()
    {
        yield break; // �ƹ��͵� ���� ����
    }
}
