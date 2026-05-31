using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollBackground : MonoBehaviour
{
    public float horizontal_speed;
    public float vertical_speed;

    private Renderer re;


    void Start()
    {
        re = GetComponent<Renderer>();
    }

    void Update()
    {
        Vector2 offset = new Vector2(Time.time * horizontal_speed, Time.time * vertical_speed);
        re.material.mainTextureOffset = offset;
    }
}
