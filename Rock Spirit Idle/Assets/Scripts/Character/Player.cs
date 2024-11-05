using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public GameObject projectilePrefab;

    private Rigidbody2D rb;
    private Collider2D coll;

    public float power = 0f;
    public float hp = 0f;
    public float maxHp;
    public float restoreHp = 0f;
    public float restoreTime = 5f;
    public float criticalRate = 0f;
    public float criticalHit = 1f;
    public float attackSpeed = 1f;
    public float attackSpeedIncrease = 0f;
    public float doubleShot = 0;
    public float berserkMultiplier = 1f; // 버서크 배율 (기본값 1, 버서크 중일 때 2로 설정)
    public float hpAmount { get { return hp / maxHp; } }

    public Image hpBar;
    public Text curHp;
    public Animator anim;
    public bool isMoving;

    private Enemy targetEnemey;

    private void Awake()
    {
        maxHp = hp;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }
    private void Start()
    {
        GameManager.Instance.player = this;
        StartCoroutine(Fire());
        StartCoroutine(Restore());
    }

    private void Update()
    {
        targetEnemey = FindClosestEnemy();
        if (targetEnemey != null)
        {
            anim.SetBool("isMoving", false);
        }
        else
        {
            anim.SetBool("isMoving", true);
        }
        hpBar.fillAmount = hpAmount;
        curHp.text = $"{hp}";
    }

    private IEnumerator Fire()
    {
        while (true)
        {
            if (targetEnemey != null && GameManager.Instance.range.canUseSkill)
            {
                yield return new WaitForSeconds(1 / (attackSpeed + attackSpeedIncrease));
                GameObject pre = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            }
            yield return null;
        }
    }

    private Enemy FindClosestEnemy()
    {
        Enemy closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (Enemy enemy in GameManager.Instance.enemies)
        {
            float distance =
                Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
    public float GetCurrentPower()
    {
        return power * berserkMultiplier; // 버서크 상태에 따라 배율 적용
    }

    public bool CriticalHit()
    {
        bool OnCritical = false;
        if (criticalRate > Random.Range(0, 100))
        {
            OnCritical = true;
        }
        return OnCritical;
    }
    private IEnumerator Restore()
    {
        while (true)
        {
            yield return new WaitForSeconds(restoreTime);
            hp += restoreHp;
            if (hp > maxHp)
            {
                hp = maxHp;
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
        GameManager.Instance.spawner.monsterLevel = 0;

        for (int i = GameManager.Instance.enemies.Count - 1; i >= 0; i--)
        {
            GameManager.Instance.enemies[i].Die();
        }
    }
}
