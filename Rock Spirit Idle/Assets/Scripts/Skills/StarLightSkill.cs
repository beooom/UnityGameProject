using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarLightSkill : MonoBehaviour
{
    private bool isUsed = false; // ù �浹 ���� �߰� �浹�� �����ϱ� ���� ����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ù �浹�� ó���ϰ�, ���Ŀ��� ����
        if (isUsed) return;

        if (collision.TryGetComponent<Enemy>(out Enemy enemy))
        {
            isUsed = true; // ù �浹 �� �÷��� ����
            enemy.TakeDamage(GameManager.Instance.player.power * 1.5f);
            Destroy(gameObject);
        }
    }
}
