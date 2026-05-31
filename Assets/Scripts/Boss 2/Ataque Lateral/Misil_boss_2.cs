using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Misil_boss_2 : MonoBehaviour
{
    [SerializeField] public float speed = 5f; // Velocidad del misil
    private float targetX; // El punto final en el eje X
    private bool isMoving = false; // Si el misil está en movimiento
    private PoolMisil_boss2 pool; // Pool de misiles
    [SerializeField] private float health = 1f; // Vida del misil
    [SerializeField] private float damage;

    [Header("Drop Settings")]
    [SerializeField] private GameObject dropPrefab; // Prefab del ítem que se puede soltar
    [SerializeField][Range(0f, 1f)] private float dropProbability = 0.2f; // Probabilidad de soltar el ítem (0.2 = 20%)

    // Método que se llama cuando el misil es lanzado
    public void SetHorizontalMovement(float side)
    {
        if (side == -1f)
        {
            targetX = 9f; // Se moverá hacia la derecha
        }
        else
        {
            targetX = -9f; // Se moverá hacia la izquierda
        }

        isMoving = true; // El misil comienza a moverse
    }

    // Método que asigna el pool al misil para devolverlo cuando termine
    public void SetPool(PoolMisil_boss2 missilePool)
    {
        pool = missilePool;
    }

    public void TakeDamage(float damage)
    {
        StartCoroutine(DamageCoroutine(damage));
    }

    private IEnumerator DamageCoroutine(float damage)
    {
        float damageDuration = 0.1f;
        health -= damage; // Reducir la salud

        if (health > 0)
        {
            // spriteRenderer.color = Color.red; // Efecto visual de daño
            yield return new WaitForSeconds(damageDuration);
            // spriteRenderer.color = Color.white; // Volver a color normal
        }
        else
        {
            DropItem(); // Intenta dropear un ítem al destruirse
            gameObject.SetActive(false);
            pool.ReturnProjectile(gameObject);
        }
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetX, transform.position.y, transform.position.z), speed * Time.deltaTime);

            if (Mathf.Abs(transform.position.x) >= Mathf.Abs(targetX))
            {
                gameObject.SetActive(false);
                pool.ReturnProjectile(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.LoseLive(1f);
            gameObject.SetActive(false);
            pool.ReturnProjectile(gameObject);
            // Aquí podrías instanciar la animación de impacto
        }
    }

    // Restablecer la vida del misil cuando se active desde el pool
    private void OnEnable()
    {
        health = 1f; // Restablece la vida cada vez que el misil se reutiliza desde el pool
    }

    private void DropItem()
    {
        if (dropPrefab != null && Random.value <= dropProbability) // Comprueba si hay un prefab y si supera la probabilidad
        {
            Instantiate(dropPrefab, transform.position, Quaternion.identity); // Instancia el ítem en la posición del misil
        }
    }
}
