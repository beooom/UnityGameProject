using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

public class basicProjectile : MonoBehaviour
{
    private Enemy target;
    public AnimationCurve curve;

    public float flightSpeed;
    public float hoverHeight;

    private bool isUsed = false; // ù �浹 ���� �߰� �浹�� �����ϱ� ���� ����

    private void Start()
    {
        FindCloseEnemy();
        // Ÿ���� ��ȿ�� ���� Fire �ڷ�ƾ ����
        if (target != null)
        {
            StartCoroutine(Fire());
        }
        else
        {
            Destroy(gameObject); // Ÿ���� ������ ����ü ����
        }
    }
    private IEnumerator Fire()
    {
        if (target == null)
        {
            Destroy(gameObject); // Ÿ���� ������ ����ü�� ����
            yield break;
        }

        float duration = flightSpeed;
        float time = 0.0f;
        Vector3 start = GameManager.Instance.player.transform.position;
        Vector3 end = target.transform.position;

        if (target != null)
        {
            while (time < duration)
            {
                time += Time.deltaTime;
                float linearT = time / duration;
                float heightT = curve.Evaluate(linearT);

                float height = Mathf.Lerp(0.0f, hoverHeight, heightT);

                transform.position = Vector2.Lerp(start, end, linearT) + new Vector2(0.0f, height);

                yield return null;
            }
        }
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ù �浹�� ó���ϰ�, ���Ŀ��� ����
        if (isUsed) return;

        if (collision.TryGetComponent<Enemy>(out Enemy enemy))
        {
            isUsed = true; // ù �浹 �� �÷��� ����
            if (GameManager.Instance.player.CriticalHit())
                enemy.TakeDamage(GameManager.Instance.player.GetCurrentPower() * GameManager.Instance.player.criticalHit);
            else
                enemy.TakeDamage(GameManager.Instance.player.GetCurrentPower());
            Destroy(gameObject);
        }
    }

    private void FindCloseEnemy()
    {
        target = null; //������� ������ ��
        float targetDistance = float.MaxValue; //������ �Ÿ�

        foreach (Enemy enemy in GameManager.Instance.enemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, transform.position);
            if (distance < targetDistance) //������ ���� ������ ������
            {
                targetDistance = distance;
                target = enemy;
            }
        }
    }
}
