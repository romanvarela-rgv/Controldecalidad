using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EnemyFlyThrough : MonoBehaviour
{
    [SerializeField] private HealthbarEnemyFlyThrough healthbar;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float maxHealth = 100f;
    private float health;

    public int scoreValue = 15;  // Puntaje al destruir
    public float moveSpeed = 5f;
    public float zigzagFrequency = 2f;    // Velocidad de oscilación
    public float zigzagAmplitude = 0.5f;  // Amplitud del zigzag
    public GameObject bulletPrefab;

    private Vector2 moveDirection = Vector2.down; // Dirección inicial (por defecto hacia abajo)
    private float startAxisPosition; // Posición inicial para el zigzag

    [SerializeField] private float dropProbability; // Probabilidad de que el enemigo suelte un objeto (30% en este caso)
    [SerializeField] private GameObject dropItemPrefab; // Prefab del objeto que puede soltar

    [SerializeField] private GameObject deattheffect;

    public AudioSource Clip;

    public Color enemyColor = Color.white;


    private void Start()
    {
        health = maxHealth;
        healthbar.UpdateHealthbar(maxHealth, health);
        spriteRenderer = GetComponent<SpriteRenderer>();

        startAxisPosition = moveDirection == Vector2.down ? transform.position.x : transform.position.y;

        // Dispara una bala al frente en intervalos
        InvokeRepeating("Shoot", 1f, 3f);

        // Autodestrucción después de 8 segundos
        Invoke("SelfDestruct", 8f);
    }

    private void Update()
    {
        // Movimiento en la dirección definida con un zigzag en el eje perpendicular
        if (moveDirection == Vector2.down) // Si se mueve hacia abajo
        {
            float xOffset = Mathf.Sin(Time.time * zigzagFrequency) * zigzagAmplitude;
            transform.position = new Vector2(startAxisPosition + xOffset, transform.position.y - moveSpeed * Time.deltaTime);
        }
        else // Movimiento horizontal
        {
            float yOffset = Mathf.Sin(Time.time * zigzagFrequency) * zigzagAmplitude;
            transform.position = new Vector2(transform.position.x + moveDirection.x * moveSpeed * Time.deltaTime, startAxisPosition + yOffset);
        }
    }

    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
    }

    private void Shoot()
    {
        if (bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = moveDirection * 10f;  // Bala sigue dirección de movimiento
            Destroy(bullet, 5f);  // Destruir bala fuera de pantalla
        }
    }

    private void SelfDestruct()
    {
        if (this != null)  // Verifica que el objeto aún exista antes de destruirlo
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        StartCoroutine(DamageCoroutine(damage));
    }

    private IEnumerator DamageCoroutine(float damage)
    {
        health -= damage;
        healthbar.UpdateHealthbar(maxHealth, health);

        if (health > 0)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
        else
        {
            // Crear la animación de muerte SOLO UNA VEZ
            GameObject deathEffect = Instantiate(deattheffect, transform.position, Quaternion.identity);

            // Cambiar el color del efecto de muerte
            SpriteRenderer deathSprite = deathEffect.GetComponent<SpriteRenderer>();
            if (deathSprite != null)
            {
                deathSprite.color = enemyColor;
            }

            Clip.Play();
            yield return new WaitForSeconds(0.35f);

            Destroy(gameObject); // Destruir el enemigo
            GameManager.Instance.AddScore(scoreValue);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            TryDropItem();
            GameManager.Instance.LoseLive(0.2f);
            Destroy(gameObject);
        }
    }


    private void TryDropItem()
    {
        // Generar un número aleatorio entre 0 y 1
        float randomValue = Random.value;

        // Verificar si el número generado está dentro de la probabilidad de drop
        if (randomValue <= dropProbability)
        {
            // Instanciar el objeto en la posición del enemigo
            Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
        }
    }

    /*

    private void OnDestroy()
    {
        FindObjectOfType<EnemyFlyThroughSpawner>().EnemyDestroyed();
    }

    */
}
