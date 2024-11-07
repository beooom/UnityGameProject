using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarLightProjectile : MonoBehaviour
{
    public float damageMultiplier = 1.5f;
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
            StartCoroutine(destroy(gameObject));
        }
    }
    public IEnumerator destroy(GameObject star)
    {
        coll.enabled = false;
        anim.gameObject.SetActive(true); 
        yield return new WaitForSeconds(0.3f); // 애니메이션 대기 시간

        Destroy(star);
    }
}
