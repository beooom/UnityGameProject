using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class StarLightSkill : SkillBase
{
    public GameObject starlightPrefab; // ��Ÿ����Ʈ ����ü ������
    public int maxStarlights = 10; // �ִ� ��Ÿ����Ʈ ��
    public float fireInterval = 0.5f; // ����ü �߻� ����
    public float projectileSpeed = 5f; // ����ü �ӵ�
    public float upwardDistance = 2f; // �ʱ� ��� �Ÿ�

    protected override IEnumerator ExecuteSkill(Enemy target)
    {
        for (int i = 0; i < maxStarlights; i++)
        {
            Vector2 spawnPosition = player.transform.position;
            GameObject starlight = Instantiate(starlightPrefab, spawnPosition, Quaternion.identity);
            GameManager.Instance.starlightProjectiles.Add(starlight);

            float angle = Random.Range(55f, 110f);
            Vector2 randomDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector2 upwardPosition = spawnPosition + randomDirection * upwardDistance;
            StartCoroutine(MoveUpwardsAndAim(starlight, upwardPosition));

            yield return new WaitForSeconds(fireInterval);
        }
    }

    private IEnumerator MoveUpwardsAndAim(GameObject starlight, Vector2 upwardPosition)
    {
        float upwardSpeed = projectileSpeed / 3.5f; // �ʱ� ��� �ӵ�

        // �÷��̾� �Ӹ� ���� �̵�
        while (starlight != null && (Vector2)starlight.transform.position != upwardPosition)
        {
            starlight.transform.position = Vector2.MoveTowards(starlight.transform.position, upwardPosition, upwardSpeed * Time.deltaTime);
            yield return null;
        }

        if (starlight != null)
        {
            StartCoroutine(EnemyAim(starlight));
        }
    }

    private IEnumerator EnemyAim(GameObject starlight)
    {
        while (starlight != null) // ����ü�� �����ϴ� ���� �ݺ�
        {
            Enemy closestEnemy = FindClosestEnemy();

            // ���� �����ϰ� ��ų ��� ���� ���¸� �߻�
            if (closestEnemy != null && GameManager.Instance.range.canUseSkill)
            {
                yield return MoveProjectile(starlight, closestEnemy.transform.position);
                GameManager.Instance.starlightProjectiles.Remove(starlight);
                yield break; // �߻簡 �Ϸ�Ǹ� �ڷ�ƾ ����
            }

            // ���� ������ ���� ������ ���
            yield return null;
        }
    }

    private IEnumerator MoveProjectile(GameObject starlight, Vector2 targetPosition)
    {
        StarLightProjectile projectileScript = starlight.gameObject.GetComponent<StarLightProjectile>();
        while (starlight != null && (Vector2)starlight.transform.position != targetPosition)
        {
            starlight.transform.position = Vector2.MoveTowards(starlight.transform.position, targetPosition, projectileSpeed * Time.deltaTime);
            yield return null;
        }

        if (starlight != null)
        {
            GameManager.Instance.starlightProjectiles.Remove(starlight);
            StartCoroutine(projectileScript.destroy(starlight));
        }
    }
}
