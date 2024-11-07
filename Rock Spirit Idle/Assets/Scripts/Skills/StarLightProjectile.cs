using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarLightProjectile : MonoBehaviour
{
    public float damageMultiplier = 1.5f; // 플레이어 공격력의 500% 데미지
    private bool isUsed = false; // 첫 충돌 이후 추가 충돌을 방지하기 위한 변수

    private Collider2D coll;
    public Animator anim;
    private void Awake()
    {
        coll = GetComponent<Collider2D>();
        anim.gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 첫 충돌만 처리하고, 이후에는 무시
        if (isUsed) return;

        if (collision.TryGetComponent<Enemy>(out Enemy enemy))
        {
            float damage = GameManager.Instance.player.GetCurrentPower() * damageMultiplier;
            isUsed = true; // 첫 충돌 후 플래그 설정
            if (GameManager.Instance.player.CriticalHit())
            {
                damage *= GameManager.Instance.player.criticalHit;
            }
            enemy.TakeDamage(damage);
            GameManager.Instance.RemoveProjectile(gameObject);
            StartCoroutine(destroy());
            //Destroy(gameObject);
        }
    }
    private IEnumerator destroy()
    {
        coll.enabled = false; // 충돌 비활성화
        anim.gameObject.SetActive(true); // 애니메이션 활성화

        yield return new WaitForSeconds(0.8f); // 애니메이션 대기 시간

        Destroy(gameObject); // 애니메이션 종료 후 객체 파괴
    }
}
