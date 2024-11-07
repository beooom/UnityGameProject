using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarLightProjectile : MonoBehaviour
{
    public float damageMultiplier = 1.5f; // �÷��̾� ���ݷ��� 500% ������
    private bool isUsed = false; // ù �浹 ���� �߰� �浹�� �����ϱ� ���� ����

    private Collider2D coll;
    public Animator anim;
    private void Awake()
    {
        coll = GetComponent<Collider2D>();
        anim.gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ù �浹�� ó���ϰ�, ���Ŀ��� ����
        if (isUsed) return;

        if (collision.TryGetComponent<Enemy>(out Enemy enemy))
        {
            float damage = GameManager.Instance.player.GetCurrentPower() * damageMultiplier;
            isUsed = true; // ù �浹 �� �÷��� ����
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
        coll.enabled = false; // �浹 ��Ȱ��ȭ
        anim.gameObject.SetActive(true); // �ִϸ��̼� Ȱ��ȭ

        yield return new WaitForSeconds(0.8f); // �ִϸ��̼� ��� �ð�

        Destroy(gameObject); // �ִϸ��̼� ���� �� ��ü �ı�
    }
}
