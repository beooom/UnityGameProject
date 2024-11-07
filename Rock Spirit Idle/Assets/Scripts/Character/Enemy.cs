using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private Transform target;
    private Rigidbody2D rb;
    private Collider2D coll;

    public float hp = 1;
    public float maxHp;
    public float power = 1;
    public float attackSpeed = 0.7f;
    bool isDead = false;
    private Coroutine MoveCo;
    public float hpAmount { get { return hp / maxHp; } }

    public Image hpBar;
    public Animator anim;
    private void Awake()
    {
        maxHp = hp;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }
    private IEnumerator Start()
    {
        GameManager.Instance.enemies.Add(this);

        yield return null;

        target = GameManager.Instance.player.transform;
        MoveCo = StartCoroutine(Move());
        StartCoroutine(CheckEnemyDeath());
    }

    private void Update()
    {
        if (isDead) return;
        hpBar.fillAmount = hpAmount;
    }
    private IEnumerator CheckEnemyDeath()
    {
        while (true)
        {
            if (hp <= 0)
            {
                DataManager.Instance.totalGold += 10;
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
            anim.SetBool("Attack", true);
            StartCoroutine(Attack());
        }
    }
    private IEnumerator Attack()
    {
        while(true)
        {
            yield return new WaitForSeconds(attackSpeed);
            GameManager.Instance.player.TakeDamage(power);
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
    }

    public void Die()
    {
        GameManager.Instance.enemies.Remove(this);
        Destroy(gameObject);
    }

}
