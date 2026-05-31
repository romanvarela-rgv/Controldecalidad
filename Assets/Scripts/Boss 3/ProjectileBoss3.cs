using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBoss3 : MonoBehaviour
{

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.LoseLive(1.5f);
            Destroy(gameObject);
        }

    }
}
