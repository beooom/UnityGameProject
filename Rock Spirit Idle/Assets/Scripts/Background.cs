using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private MeshRenderer render;
    private float offset;
    [Tooltip("background �̵��ӵ�")]
    public float flowSpeed = 0.06f;

    public Player player;

    private void Start()
    {
        render = GetComponent<MeshRenderer>();
    }
    void Update()
    {
        offset += Time.deltaTime * flowSpeed;
        render.material.mainTextureOffset = new Vector2(offset, 0);
        //transform.Translate(Vector3.left * Time.deltaTime * flowSpeed);
        //if (transform.position.x < -4.5f)
        //    transform.position = Vector3.right * 4.5f;
        if (player.anim.GetBool("isMoving") == false)
            flowSpeed = 0f;
        else 
            flowSpeed = 0.06f;

    }
}
