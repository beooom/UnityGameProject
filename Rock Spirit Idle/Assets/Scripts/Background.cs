using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [Tooltip("background 이동속도")]
    public float flowSpeed = 0.8f;

    public  Player player;

    void Update()
    {
        transform.Translate(Vector3.left * Time.deltaTime * flowSpeed);
        if (transform.position.x < -5f)
            transform.position = Vector3.right * 5f;
        if (player.anim.GetBool("isMoving") == false)
            flowSpeed = 0f;
        else 
            flowSpeed = 0.8f;

    }
}
