using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
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
    public Animator re;
    public bool isMoving;
    public bool isDeath = false;

    private Enemy targetEnemy;
    public BasicAttack basicAttack;

    private void Awake()
    {
        maxHp = hp;
        GameManager.Instance.player = this;
    }
    private void Start()
    {
        re.gameObject.SetActive(false);
        StartCoroutine(Restore());
        basicAttack = GetComponentInChildren<BasicAttack>();
    }

    private void Update()
    {
        targetEnemy = FindClosestEnemy();
        if (targetEnemy != null && GameManager.Instance.range.canUseSkill)
        {
            anim.SetBool("isMoving", false);
            if (GameManager.Instance.range.canUseSkill && !basicAttack.isCooldown)
            {
                basicAttack.TriggerSkill(targetEnemy);
            }
        }
        else if (targetEnemy == null)
        {
            anim.SetBool("isMoving", true);
        }
        hpBar.fillAmount = hpAmount;
        curHp.text = $"{hp}";
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
        return criticalRate > Random.Range(0, 100);
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
            StartCoroutine(Die());
        }
    }

    public IEnumerator Die()
    {
        // 2초 대기 후 게임 재시작 (이 때 실시간 기준으로 대기)
        GameManager.Instance.currentState = GameState.GameOver;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.1f);

        GameManager.Instance.Restart();
        GameManager.Instance.player.hp = GameManager.Instance.player.maxHp;

        re.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        re.gameObject.SetActive(false);
    }
}
