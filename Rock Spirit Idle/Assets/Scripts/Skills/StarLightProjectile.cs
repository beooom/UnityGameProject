using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarLightProjectile : MonoBehaviour
{
    public float damageMultiplier = 1.5f;
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
            StartCoroutine(destroy(gameObject));
        }
    }
    public IEnumerator destroy(GameObject star)
    {
        coll.enabled = false;
        anim.gameObject.SetActive(true); 
        yield return new WaitForSeconds(0.3f); // �ִϸ��̼� ��� �ð�

        Destroy(star);
    }
}
