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
    // 공격 속도에 따라 지속적으로 공격하는 코루틴
    protected override IEnumerator ExecuteSkill(Enemy target)
    {
        while (true)
        {
            target = FindClosestEnemy();
            if (target != null && GameManager.Instance.range.canUseSkill)
            {
                FireProjectile(target);
                // 더블샷 조건 확인 후 추가 공격 수행
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
        yield break; // 아무것도 하지 않음
    }
}
