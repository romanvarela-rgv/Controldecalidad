using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBoss : MonoBehaviour
{
    public float speed;
    private float waitTime;
    public float startWaitTime;

    public Transform[] MoveBoss3;
    public int targetPoint;
    private int randomSpot;



    void Start()
    {
        targetPoint = 0;
        waitTime = startWaitTime;
        randomSpot = Random.Range(0, MoveBoss3.Length);
    }



    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, MoveBoss3[randomSpot].position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, MoveBoss3[randomSpot].position) < 0.2f)
        {
            if (waitTime <= 0)
            {
                randomSpot = Random.Range(0, MoveBoss3.Length);
                waitTime = startWaitTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }
}
