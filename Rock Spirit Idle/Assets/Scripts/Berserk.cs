using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berserk : MonoBehaviour
{
    public Animator anim;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.8f);
        anim.SetBool("berserking", true);
    }

}
