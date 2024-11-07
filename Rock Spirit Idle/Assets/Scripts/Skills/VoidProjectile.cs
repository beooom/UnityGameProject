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

    private bool isExploding = false;

    public Animator anim;
    private Camera mainCamera;
    private Collider2D coll;

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
        if (!isExploding)
        {
            transform.Translate(Vector3.right * voidSpeed * Time.deltaTime);
        }
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);

        // 시야를 벗어났거나 관통 횟수가 0이 되면 폭발 코루틴 시작
        if ((viewPos.x > 1 || pierceCount <= 0) && !isExploding)
        {
            StartCoroutine(ExplodeAndDestroy());
        }
    }
    private IEnumerator ExplodeAndDestroy()
    {
        isExploding = true; // 이동을 멈추기 위해 플래그 설정
        anim.SetTrigger("Explode"); // 폭발 애니메이션 재생

        yield return new WaitForSeconds(0.5f); // 폭발 애니메이션 재생 시간 대기

        Destroy(gameObject); // 애니메이션 종료 후 객체 파괴
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
            {
                damage *= GameManager.Instance.player.criticalHit;
            }
            collision.GetComponent<Enemy>().TakeDamage(damage);
            pierceCount--;
            preDamageTime = Time.time;
        }
    }
}
