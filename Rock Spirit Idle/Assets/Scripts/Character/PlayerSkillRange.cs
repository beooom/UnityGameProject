using System.Collections;
using System.Collections.Generic;
using Unity.Content;
using UnityEngine;

public class PlayerSkillRange : MonoBehaviour
{
    public bool canUseSkill = false;

    private void Start()
    {
        GameManager.Instance.range = this;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            canUseSkill = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            canUseSkill = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        canUseSkill = false;
    }
}