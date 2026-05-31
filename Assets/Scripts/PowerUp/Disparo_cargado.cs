using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disparo_cargado : MonoBehaviour
{
   
   [SerializeField] private float bulletSpeed = 15f;
    private float damage = 50f;  // Mayor daño para el disparo cargado
   [SerializeField] private float lifespan = 3f; // Duración de vida del disparo cargado

    private void Start()
    {
        // Destruye el disparo después de su tiempo de vida
        Destroy(gameObject, lifespan);
    }

    public void Fire(Vector2 direction)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction * bulletSpeed;
    }

   
}

