using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarLightProjectile : MonoBehaviour
{
    public float damageMultiplier = 1.5f; // �÷��̾� ���ݷ��� 500% ������
    private bool isUsed = false; // ù �浹 ���� �߰� �浹�� �����ϱ� ���� ����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ù �浹�� ó���ϰ�, ���Ŀ��� ����
        if (isUsed) return;

        if (collision.TryGetComponent<Enemy>(out Enemy enemy))
        {
            float damage = GameManager.Instance.player.GetCurrentPower() * damageMultiplier;
            isUsed = true; // ù �浹 �� �÷��� ����
            if (GameManager.Instance.player.CriticalHit())
                enemy.TakeDamage(damage * GameManager.Instance.player.criticalHit);
            else
                enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
