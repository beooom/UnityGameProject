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

    private bool isUsed = false; // 첫 충돌 이후 추가 충돌을 방지하기 위한 변수

    private void Start()
    {
        FindCloseEnemy();
        // 타겟이 유효할 때만 Fire 코루틴 시작
        if (target != null)
        {
            StartCoroutine(Fire());
        }
        else
        {
            Destroy(gameObject); // 타겟이 없으면 투사체 제거
        }
    }
    private IEnumerator Fire()
    {
        if (target == null)
        {
            Destroy(gameObject); // 타겟이 없으면 투사체를 제거
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
        // 첫 충돌만 처리하고, 이후에는 무시
        if (isUsed) return;

        if (collision.TryGetComponent<Enemy>(out Enemy enemy))
        {
            isUsed = true; // 첫 충돌 후 플래그 설정
            if (GameManager.Instance.player.CriticalHit())
                enemy.TakeDamage(GameManager.Instance.player.GetCurrentPower() * GameManager.Instance.player.criticalHit);
            else
                enemy.TakeDamage(GameManager.Instance.player.GetCurrentPower());
            Destroy(gameObject);
        }
    }

    private void FindCloseEnemy()
    {
        target = null; //대상으로 지정된 적
        float targetDistance = float.MaxValue; //대상과의 거리

        foreach (Enemy enemy in GameManager.Instance.enemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, transform.position);
            if (distance < targetDistance) //이전에 비교한 적보다 가까우면
            {
                targetDistance = distance;
                target = enemy;
            }
        }
    }
}
