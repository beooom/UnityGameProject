using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    public GameObject projectilePrefab;

    private Rigidbody2D rb;
    private Collider2D coll;

    public float power = 1f;
    public float hp = 5f;
    public float maxHp;
    public float restoreHp = 0f;
    public float criticalRate = 0f;
    public float criticalHit = 1f;
    public float attackSpeed = 1f;
    public float doubleShot = 0;

    public Animator anim;
    public bool isMoving;
    Enemy targetEnemey;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }
    private void Start()
    {
        GameManager.Instance.player = this;
        StartCoroutine(Fire());
    }

    private void Update()
    {
        FindCloseEnemy();
        if (targetEnemey != null)
        {
            anim.SetBool("isMoving", false);
        }
        else
        {
            anim.SetBool("isMoving", true);
        }
    }

    private IEnumerator Fire()
    {
        while (true)
        {
            if (targetEnemey != null)
            {
                yield return new WaitForSeconds(attackSpeed);
                GameObject pre = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            }
            yield return null;
        }
    }

    private void FindCloseEnemy()
    {
        targetEnemey = null; //대상으로 지정된 적
        float targetDistance = float.MaxValue; //대상과의 거리

        foreach (Enemy enemy in GameManager.Instance.enemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, transform.position);
            if (distance < targetDistance) //이전에 비교한 적보다 가까우면
            {
                targetDistance = distance;
                targetEnemey = enemy;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {

    }
}
