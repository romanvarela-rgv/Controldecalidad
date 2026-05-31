using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class BossAttack : MonoBehaviour
{
    public Transform firePoint; // Punto de disparo
    public float projectileSpeed = 5f; // Velocidad del proyectil
    public float attackInterval; // Tiempo entre ataques
    public float waterStreamDuration = 3f; // Duración del "chorro de agua"
    public float spawnRate = 0.1f; // Tiempo entre cada proyectil del chorro

    private GameObject player;
    public ObjectPool objectPool;

    public AudioSource ClipAtack;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        objectPool = FindObjectOfType<ObjectPool>();

        if (player != null)
        {
            InvokeRepeating(nameof(StartWaterAttack), attackInterval, attackInterval);
        }
        else
        {
            Debug.LogError("Player no encontrado. Asegúrate de que tenga la etiqueta 'Player'.");
        }
    }

    private void StartWaterAttack()
    {
        if (player != null)
        {
  
            StartCoroutine(WaterStream());
        }
    }

    private IEnumerator WaterStream()
    {
        float elapsedTime = 0f;

        while (elapsedTime < waterStreamDuration)
        {
            ClipAtack.Play();
            SpawnProjectile();
            yield return new WaitForSeconds(spawnRate);
            elapsedTime += spawnRate;
        }
    }

    private void SpawnProjectile()
    {
        if (firePoint != null && player != null && objectPool != null)
        {
            GameObject projectile = objectPool.GetObject();
            projectile.transform.position = firePoint.position;
            projectile.transform.rotation = Quaternion.identity;

            Vector2 direction = (player.transform.position - firePoint.position).normalized;

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * projectileSpeed;
            }

            // Devolver el proyectil al pool después de 3 segundos
            StartCoroutine(ReturnToPool(projectile, 3f));
        }
    }

    private IEnumerator ReturnToPool(GameObject projectile, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (objectPool != null)
        {
            objectPool.ReturnObject(projectile);
        }
    }
}
