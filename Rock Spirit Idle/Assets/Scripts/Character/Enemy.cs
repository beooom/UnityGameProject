using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    private Transform target;
    private Rigidbody2D rb;
    private Collider2D coll;

    //public float StopPoint = 3f;
    public float hp = 1;
    public float maxHp;
    public float power = 1;
    bool isDead = false;
    private Coroutine MoveCo;

    public Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }
    private IEnumerator Start()
    {
        GameManager.Instance.enemies.Add(this); //적 리스트에 자기 자신을 Add
        maxHp = hp;

        yield return null;//한프레임 쉬자.

        target = GameManager.Instance.player.transform;
        MoveCo = StartCoroutine(Move());
        StartCoroutine(CheckEnemyDeath());
    }

    private void Update()
    {
        if (isDead) return;
    }
    private IEnumerator CheckEnemyDeath()
    {
        while (true)
        {
            if (hp <= 0)
            {
                isDead = true;
                anim.SetTrigger("Die");
                coll.enabled = false;
                yield return new WaitForSeconds(0.8f);
                Die();
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator Move()
    {
        while (true)
        {
            yield return null;
            Vector2 moveDir = target?.position - transform.position ?? Vector2.zero;
            moveDir.y = 0;
            moveDir = moveDir.normalized;
            if (moveDir.magnitude > 0.0000f)
            {
                Vector2 movePos = rb.position + (moveDir * Time.fixedDeltaTime);
                rb.MovePosition(movePos);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopCoroutine(MoveCo);
            TakeDamage(1f);
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        print($"{damage} 아야!");
    }

    public void Die()
    {
        DataManager.Instance.totalGold += 10;
        GameManager.Instance.enemies.Remove(this);
        Destroy(gameObject);
    }

}
