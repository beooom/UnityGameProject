using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class StarLightSkill : SkillBase
{
    public GameObject starlightPrefab; // 스타라이트 투사체 프리팹
    public int maxStarlights = 10; // 최대 스타라이트 수
    public float fireInterval = 0.5f; // 투사체 발사 간격
    public float projectileSpeed = 5f; // 투사체 속도
    public float upwardDistance = 2f; // 초기 상승 거리

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
        float upwardSpeed = projectileSpeed / 3.5f; // 초기 상승 속도

        // 플레이어 머리 위로 이동
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
        while (starlight != null) // 투사체가 존재하는 동안 반복
        {
            Enemy closestEnemy = FindClosestEnemy();

            // 적이 존재하고 스킬 사용 가능 상태면 발사
            if (closestEnemy != null && GameManager.Instance.range.canUseSkill)
            {
                yield return MoveProjectile(starlight, closestEnemy.transform.position);
                GameManager.Instance.starlightProjectiles.Remove(starlight);
                yield break; // 발사가 완료되면 코루틴 종료
            }

            // 적이 없으면 다음 프레임 대기
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
