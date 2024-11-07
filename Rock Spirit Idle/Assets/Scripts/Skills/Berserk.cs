using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Berserk : SkillBase
{
    public float durationTime = 10f; //지속시간
    public float powerUP = 2f; // 공격력 업할 수치

    public Animator anim;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override IEnumerator ExecuteSkill(Enemy target)
    {
        anim.SetBool("berserking", true);
        GameManager.Instance.player.berserkMultiplier = powerUP;

        yield return new WaitForSeconds(durationTime);

        anim.SetBool("berserking", false);
        GameManager.Instance.player.berserkMultiplier = 1f;
    }
}
