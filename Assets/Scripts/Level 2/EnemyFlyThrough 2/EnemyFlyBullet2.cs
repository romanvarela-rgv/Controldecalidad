using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlyBullet2 : MonoBehaviour
{
    public float speed = 10f;  // Velocidad de la bala
    private Vector2 moveDirection = Vector2.down; // Dirección inicial hacia abajo (modificable si es necesario)

    // Establece la dirección de movimiento de la bala
    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
    }

    private void Start()
    {
        // Si no se establece una dirección, la bala irá hacia abajo por defecto
        if (moveDirection == Vector2.zero)
        {
            moveDirection = Vector2.down;
        }
    }

    private void Update()
    {
        // Movimiento de la bala en la dirección establecida con la velocidad configurada
        transform.Translate(moveDirection * speed * Time.deltaTime);

        // Destruir la bala si sale de la pantalla
        Vector3 screenPosition = Camera.main.WorldToViewportPoint(transform.position);
        if (screenPosition.y < 0 || screenPosition.y > 1 || screenPosition.x < 0 || screenPosition.x > 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!GameManager.Instance.IsPlayerInvulnerable())
            {
                GameManager.Instance.LoseLive(0.4f);
            }
            Destroy(gameObject);
        }
    }
}    
