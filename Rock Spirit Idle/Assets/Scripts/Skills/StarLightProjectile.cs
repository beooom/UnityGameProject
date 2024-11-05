using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarLightProjectile : MonoBehaviour
{
    public float damageMultiplier = 1.5f; // 플레이어 공격력의 500% 데미지
    private bool isUsed = false; // 첫 충돌 이후 추가 충돌을 방지하기 위한 변수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 첫 충돌만 처리하고, 이후에는 무시
        if (isUsed) return;

        if (collision.TryGetComponent<Enemy>(out Enemy enemy))
        {
            float damage = GameManager.Instance.player.GetCurrentPower() * damageMultiplier;
            isUsed = true; // 첫 충돌 후 플래그 설정
            if (GameManager.Instance.player.CriticalHit())
                enemy.TakeDamage(damage * GameManager.Instance.player.criticalHit);
            else
                enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
