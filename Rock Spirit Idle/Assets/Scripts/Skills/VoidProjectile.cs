using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class VoidProjectile : MonoBehaviour
{
    public float damageMultiplier = 1.2f; //플레이어 공격력의 120%
    public float voidSpeed = 1f; //보이드 속도
    public int pierceCount = 10; //관통 횟수

    public float damageInterval = 0.1f; //데미지 간격
    private float preDamageTime; //이전에 데미지를 준 시간(Time.time)

    private Camera mainCamera;

    private void Awake()
    {
        preDamageTime = Time.time;
    }
    void Start()
    {
        mainCamera = Camera.main; // 게임에서 메인 카메라 가져오기
    }

    private void Update()
    {
        transform.Translate(Vector3.right * voidSpeed * Time.deltaTime);
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);

        // 시야 영역을 벗어나면 제거
        if (viewPos.x > 1 || pierceCount <= 0)
        {
            Destroy(gameObject);
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (preDamageTime + damageInterval > Time.time)
        {
            return;
        }
        if (collision.CompareTag("Enemy"))
        {
            float damage = GameManager.Instance.player.GetCurrentPower() * damageMultiplier;
            if (GameManager.Instance.player.CriticalHit())
                collision.GetComponent<Enemy>().TakeDamage(damage * GameManager.Instance.player.criticalHit);
            else
                collision.GetComponent<Enemy>().TakeDamage(damage);
            pierceCount--;
            preDamageTime = Time.time;
        }
    }
}
