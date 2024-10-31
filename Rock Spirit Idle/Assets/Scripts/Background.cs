using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [Tooltip("background �̵��ӵ�")]
    public float flowSpeed = 0.6f;

    public  Player player;

    void Update()
    {
        transform.Translate(Vector3.left * Time.deltaTime * flowSpeed);
        if (transform.position.x < -4.5f)
            transform.position = Vector3.right * 4.5f;
        if (player.anim.GetBool("isMoving") == false)
            flowSpeed = 0f;
        else 
            flowSpeed = 0.6f;

    }
}
