using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class StarLightProjectile : MonoBehaviour
{
    public GameObject starlightPrefab; // 스타라이트 투사체 프리팹
    public int maxStarlights = 10; // 최대 스타라이트 수
    public float starlightCooldown = 7f; // 쿨타임
    public float spawnHeight = 2f; // 플레이어 위 3f 위치
    public float spawnRange = 0.5f; // 1f의 범위 내에서 랜덤 소환
    public float fireInterval = 0.5f; // 투사체 발사 간격
    public float projectileSpeed = 5f; // 투사체 속도

    private List<GameObject> starlightProjectiles = new List<GameObject>(); // 생성된 스타라이트 투사체 리스트
    private bool isCooldown = false; // 쿨타임 상태 확인용

    public Image cooldownImage; // 쿨타임을 표시할 이미지
    public Text cooldownText; // 쿨타임을 표시할 텍스트

    private void Awake()
    {
        cooldownImage.fillAmount = 0f; // 초기화 - 쿨타임이 없을 때 이미지 비워둠
    }
    private void Start()
    {
        StartCoroutine(StarlightSkillRoutine());
    }
    private IEnumerator StarlightSkillRoutine()
    {
        while (true)
        {
            Enemy enemy = FindClosestEnemy();
            if (enemy != null && !isCooldown)
            {
                // 쿨타임 시작
                isCooldown = true;
                StartCoroutine(CooldownRoutine()); // 쿨타임 표시 코루틴 시작

                for (int i = 0; i < maxStarlights; i++)
                {
                    Vector2 randCircle = Random.insideUnitCircle * spawnRange;
                    Vector2 spawnPosition =
                        new Vector2(GameManager.Instance.player.transform.position.x,
                        GameManager.Instance.player.transform.position.y + spawnHeight) + randCircle;
                    GameObject starlight = Instantiate(starlightPrefab, spawnPosition, Quaternion.identity);
                    starlightProjectiles.Add(starlight);
                }

                // 투사체가 있을 때마다 발사
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
    private IEnumerator CooldownRoutine()
    {
        float elapsed = 0f;
        cooldownImage.fillAmount = 1f; // 이미지가 완전히 채워진 상태로 시작

        while (elapsed < starlightCooldown)
        {
            elapsed += Time.deltaTime;
            cooldownImage.fillAmount = 1 - (elapsed / starlightCooldown); // 시간이 지남에 따라 이미지가 비워짐
            yield return null;
        }

        cooldownImage.fillAmount = 0f; // 쿨타임이 완료되면 이미지 초기화
        isCooldown = false;
    }

    private Enemy FindClosestEnemy()
    {
        Enemy closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (Enemy enemy in GameManager.Instance.enemies)
        {
            float distance = Vector3.Distance(GameManager.Instance.player.transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
}
