using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    private Enemy target;
    public AnimationCurve curve;
    public float flightSpeed;
    public float hoverHeight;

    private bool isUsed = false; // ù �浹�� ó��
    public void SetTarget(Enemy newTarget)
    {
        if (newTarget == null)
        {
            Destroy(gameObject); // Ÿ���� ������ ����ü ����
            return;
        }
        target = newTarget;
        StartCoroutine(FireCurve());
    }

    private IEnumerator FireCurve()
    {
        if (target == null)
        {
            Destroy(gameObject); // Ÿ���� ������ ����ü ����
            yield break;
        }

        float duration = flightSpeed;
        float time = 0.0f;
        Vector3 start = transform.position;
        Vector3 end = target.transform.position;

        while (time < duration)
        {
            time += Time.deltaTime;
            float linearT = time / duration;
            float heightT = curve.Evaluate(linearT);
            float height = Mathf.Lerp(0.0f, hoverHeight, heightT);

            transform.position = Vector2.Lerp(start, end, linearT) + new Vector2(0.0f, height);
            yield return null;
        }


        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isUsed) return;

        if (collision.TryGetComponent<Enemy>(out Enemy enemy))
        {
            isUsed = true; // ù �浹 �� �÷��� ����
            float damage = GameManager.Instance.player.GetCurrentPower();
            if (GameManager.Instance.player.CriticalHit())
            {
                damage *= GameManager.Instance.player.criticalHit;
            }
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
