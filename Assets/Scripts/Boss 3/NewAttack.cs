using System.Collections;
using UnityEngine;

public class NewAttack : MonoBehaviour
{
    public Transform firePoint; // Punto de disparo
    public float speed = 5f; // Velocidad del proyectil
    public float attackInterval = 11f; // Tiempo entre ataques

    private GameObject player;
    private ObjectPool objectPool;
    private float nextAttackTime = 11f;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        objectPool = FindObjectOfType<ObjectPool>();

        if (player == null)
        {
            Debug.LogError("Player no encontrado. Asegúrate de que tenga la etiqueta 'Player'.");
        }

        if (objectPool == null)
        {
            Debug.LogError("ObjectPool no encontrado en la escena.");
        }
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackInterval;
        }
    }

    void Attack()
    {
        if (player != null && objectPool != null)
        {
            GameObject projectile = objectPool.GetObject(true); // Usa el prefab 2
            if (projectile != null)
            {
                projectile.transform.position = firePoint.position;
                projectile.transform.rotation = Quaternion.identity;

                Vector2 direction = (player.transform.position - firePoint.position).normalized;
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    rb.velocity = direction * speed;
                }

                // Devolver al ObjectPool después de 3 segundos
                StartCoroutine(ReturnToPool(projectile, 3f));
            }
        }
    }

    private IEnumerator ReturnToPool(GameObject projectile, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (objectPool != null)
        {
            objectPool.ReturnObject(projectile, true); // Devuelve al pool de prefab 2
        }
    }
}
