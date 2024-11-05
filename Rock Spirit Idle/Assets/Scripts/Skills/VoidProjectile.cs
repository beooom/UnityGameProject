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

    private Camera mainCamera;

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
        transform.Translate(Vector3.right * voidSpeed * Time.deltaTime);
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);

        // �þ� ������ ����� ����
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
