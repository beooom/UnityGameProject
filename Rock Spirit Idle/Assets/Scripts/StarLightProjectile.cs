using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarLightProjectile : MonoBehaviour
{
    public GameObject starlightPrefab; // 스타라이트 투사체 프리팹
    public int maxStarlights = 10; // 최대 스타라이트 수
    public float starlightCooldown = 5f; // 쿨타임
    public float spawnHeight = 3f; // 플레이어 위 3f 위치
    public float spawnRange = 1f; // 1f의 범위 내에서 랜덤 소환
    public float fireInterval = 0.5f; // 투사체 발사 간격
    public float projectileSpeed = 5f; // 투사체 속도

    private Transform playerTransform;
    private List<GameObject> starlightProjectiles = new List<GameObject>(); // 생성된 스타라이트 투사체 리스트
    private bool isCooldown = false; // 쿨타임 상태 확인용


    private IEnumerator Start()
    {
        yield return null;
        playerTransform = GameManager.Instance.player.transform;
        StartCoroutine(StarlightSkillRoutine());
    }

    private IEnumerator StarlightSkillRoutine()
    {
        while (true)
        {
            if (!isCooldown && starlightProjectiles.Count == 0) // 쿨타임이 끝나고 모든 투사체가 소진되었을 때
            {
                yield return new WaitForSeconds(starlightCooldown); // 쿨타임 대기
                isCooldown = true;

                // 새로운 Starlight 생성
                for (int i = 0; i < maxStarlights; i++)
                {
                    Vector2 randCircle = Random.insideUnitCircle;
                    Vector2 spawnPosition = 
                        new Vector2(playerTransform.position.x, playerTransform.position.y + spawnHeight) + randCircle;
                    GameObject starlight = Instantiate(starlightPrefab, spawnPosition, Quaternion.identity);
                    starlightProjectiles.Add(starlight);
                }
            }

            // 투사체가 있을 때마다 발사
            if (starlightProjectiles.Count > 0)
            {
                yield return StartCoroutine(FireStarlight());
            }

            yield return null;
        }
    }

    private IEnumerator FireStarlight()
    {
        while (starlightProjectiles.Count > 0)
        {
            Enemy closestEnemy = FindClosestEnemy();
            if (closestEnemy != null)
            {
                // 첫 번째 Starlight를 발사하고 리스트에서 제거
                GameObject starlight = starlightProjectiles[0];
                starlightProjectiles.RemoveAt(0);

                StartCoroutine(MoveProjectile(starlight, closestEnemy.transform.position));

                yield return new WaitForSeconds(fireInterval); // 발사 간격
            }
            else
            {
                yield return null; // 적이 없으면 대기
            }
        }

        // 모든 투사체 발사 완료 후 쿨타임 초기화
        isCooldown = false;
    }

    private IEnumerator MoveProjectile(GameObject starlight, Vector2 targetPosition)
    {
        while (starlight != null && (Vector2)starlight.transform.position != targetPosition)
        {
            starlight.transform.position = Vector2.MoveTowards(starlight.transform.position, targetPosition, projectileSpeed * Time.deltaTime);
            yield return null;
        }

        if (starlight != null)
        {
            Destroy(starlight); // 목표 지점 도달 시 파괴
        }
    }

    private Enemy FindClosestEnemy()
    {
        Enemy closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (Enemy enemy in GameManager.Instance.enemies)
        {
            float distance = Vector3.Distance(playerTransform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
}
