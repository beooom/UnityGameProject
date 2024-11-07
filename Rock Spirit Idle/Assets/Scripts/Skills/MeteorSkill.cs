using System.Collections;
using UnityEngine;

public class MeteorSkill : SkillBase
{
    public GameObject meteorPrefab;
    public Vector3 spawnPos = Vector3.up * 1f;
    public float explosionRadius = 2f;
    public float damageMultiplier = 12f;

    protected override IEnumerator ExecuteSkill(Enemy target)
    {
        Vector3 spawnPosition = target.transform.position + spawnPos;
        GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);

        yield return new WaitForSeconds(1.82f);

        Explode(meteor.transform.position);
        Destroy(meteor);
    }

    private void Explode(Vector3 explosionPosition)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius);
        float damage = GameManager.Instance.player.GetCurrentPower() * damageMultiplier;

        foreach (Collider2D hit in hitEnemies)
        {
            if (hit.TryGetComponent<Enemy>(out Enemy enemy))
            {
                if (GameManager.Instance.player.CriticalHit())
                {
                    damage *= GameManager.Instance.player.criticalHit;
                }
                enemy.TakeDamage(damage);
            }
        }
    }
}
