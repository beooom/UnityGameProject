using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private MeshRenderer render;
    private float offset;
    [Tooltip("background 이동속도")]
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
        if (player.anim.GetBool("isMoving") == false)
            flowSpeed = 0f;
        else 
            flowSpeed = 0.06f;

    }
}
