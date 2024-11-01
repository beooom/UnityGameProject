using Lean.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidProjectile : MonoBehaviour
{
    public float damageMultiplier = 1.2f; //�÷��̾� ���ݷ��� 120%
    public float voidSpeed = 1f; //���̵� �ӵ�
    public int pierceCount = 10; //���� Ƚ��

    public float damageInterval = 0.15f; //������ ����
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
            collision.GetComponent<Enemy>().TakeDamage(GameManager.Instance.player.power * damageMultiplier);
            pierceCount--;
            print(pierceCount);
            preDamageTime = Time.time;
        }
    }
}
