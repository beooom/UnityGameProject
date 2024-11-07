using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class VoidProjectile : MonoBehaviour
{
    public float damageMultiplier = 1.2f; //�÷��̾� ���ݷ��� 120%
    public float voidSpeed = 1f; //���̵� �ӵ�
    public int pierceCount = 10; //���� Ƚ��

    public float damageInterval = 0.1f; //������ ����
    private float preDamageTime; //������ �������� �� �ð�(Time.time)

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
        mainCamera = Camera.main; // ���ӿ��� ���� ī�޶� ��������
    }

    private void Update()
    {
        if (!isExploding)
        {
            transform.Translate(Vector3.right * voidSpeed * Time.deltaTime);
        }
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);

        // �þ߸� ����ų� ���� Ƚ���� 0�� �Ǹ� ���� �ڷ�ƾ ����
        if ((viewPos.x > 1 || pierceCount <= 0) && !isExploding)
        {
            StartCoroutine(ExplodeAndDestroy());
        }
    }
    private IEnumerator ExplodeAndDestroy()
    {
        isExploding = true; // �̵��� ���߱� ���� �÷��� ����
        anim.SetTrigger("Explode"); // ���� �ִϸ��̼� ���

        yield return new WaitForSeconds(0.5f); // ���� �ִϸ��̼� ��� �ð� ���

        Destroy(gameObject); // �ִϸ��̼� ���� �� ��ü �ı�
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
