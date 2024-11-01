using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VoidSkill : MonoBehaviour
{
    public GameObject voidPrefab; //보이드 프리팹
    public float voidCooldown = 5f; //보이드 쿨타임

    public Image cooldownImage; // 쿨타임을 표시할 이미지
    public Text cooldownText; // 쿨타임을 표시할 텍스트

    private bool isCooldown = false; // 스킬이 쿨타임 상태인지 확인

    private void Awake()
    {
        cooldownImage.fillAmount = 0f; // 초기화 - 쿨타임이 없을 때 이미지 비워둠
    }

    private void Update()
    {
        if (!isCooldown)
        {
            Enemy target = FindClosestEnemy();
            if (target != null)
            {
                StartCoroutine(VoidSkillCo(target));
            }
        }
    }

    private IEnumerator VoidSkillCo(Enemy target)
    {
        isCooldown = true;
        StartCoroutine(CooldownRoutine()); // 쿨타임 표시 코루틴 시작

        Vector3 spawnPosition = GameManager.Instance.player.transform.position;
        GameObject Void = Instantiate(voidPrefab, spawnPosition, Quaternion.identity);
        yield return new WaitForSeconds(voidCooldown); // 쿨타임 대기
    }

    private IEnumerator CooldownRoutine()
    {
        float elapsed = 0f;
        cooldownImage.fillAmount = 1f; // 이미지가 완전히 채워진 상태로 시작

        while (elapsed < voidCooldown)
        {
            elapsed += Time.deltaTime;
            cooldownImage.fillAmount = 1 - (elapsed / voidCooldown); // 시간이 지남에 따라 이미지가 비워짐
            yield return null;
        }

        cooldownImage.fillAmount = 0f; // 쿨타임이 완료되면 이미지 초기화
        isCooldown = false;
    }

    private Enemy FindClosestEnemy()
    {
        Enemy closestEnemy = null;

        foreach (Enemy enemy in GameManager.Instance.enemies)
        {
            closestEnemy = enemy;
        }

        return closestEnemy;
    }
}
