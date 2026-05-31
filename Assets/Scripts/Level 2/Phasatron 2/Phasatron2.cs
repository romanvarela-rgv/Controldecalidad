using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Phasatron2 : MonoBehaviour
{
    [SerializeField] private HealthbarPhasatron healthbar;
    private float health;
    [SerializeField] public float maxHealth;
    private SpriteRenderer spriteRenderer;

    private int randomSpot;

    public float speed;
    private float waitTime;
    public float startWaitTime;

    public Transform[] moveSpots;
    public GameObject EnemyGun;

    public int valuePhasatron;

    [SerializeField] private float dropProbability; // Probabilidad de que el enemigo suelte un objeto (30% en este caso)
    [SerializeField] private GameObject dropItemPrefab; // Prefab del objeto que puede soltar
    [SerializeField] private GameObject deattheffect;

    public AudioSource Clip;

    public Color enemyColor = Color.white;
    [SerializeField] private Color deathEffectColor = Color.white; // Color del efecto de muerte asignable desde el Inspector

    void Start()
    {
        health = maxHealth;
        healthbar.UpdateHealthbar(maxHealth, health);
        waitTime = startWaitTime;
        randomSpot = Random.Range(0, moveSpots.Length);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, moveSpots[randomSpot].position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, moveSpots[randomSpot].position) < 0.2f)
        {
            if (waitTime <= 0)
            {
                randomSpot = Random.Range(0, moveSpots.Length);
                waitTime = startWaitTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }

    // Método para recibir daño del proyectil
    public void TakeDamage(float damage)
    {
        StartCoroutine(DamageCoroutine(damage));
    }

    private IEnumerator DamageCoroutine(float damage)
    {
        float damageDuration = 0.1f;
        health -= damage; // Reducir la salud
        healthbar.UpdateHealthbar(maxHealth, health);

        if (health > 0)
        {
            Clip.Play(); // Reproducir el sonido de daño
            spriteRenderer.color = Color.red; // Efecto visual de daño
            yield return new WaitForSeconds(damageDuration); // Esperar un breve momento

            // Volver al color original especificado en hexadecimal
            Color normalColor;
            if (ColorUtility.TryParseHtmlString("#FFA9A9", out normalColor))
            {
                spriteRenderer.color = normalColor; // Cambiar al color original
            }
        }
        else
        {
            TryDropItem(); // Intentar soltar un ítem

            // Instanciar el efecto de muerte
            GameObject deathEffectInstance = Instantiate(deattheffect, transform.position, Quaternion.identity);

            // Ajustar el color del efecto de muerte con el color asignado en el Inspector
            SpriteRenderer deathSprite = deathEffectInstance.GetComponent<SpriteRenderer>();
            if (deathSprite != null)
            {
                deathSprite.color = deathEffectColor; // Asignar el color desde el Inspector
            }

            // Destruir al enemigo
            Destroy(gameObject);

            // Agregar puntuación
            GameManager.Instance.AddScore(valuePhasatron);
        }

        yield return null; // Asegurarse de que la corutina complete su ejecución
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.LoseLive(1);
        }
    }

    private void OnDestroy()
    {
        var spawner = FindObjectOfType<Phasatron2Spawner>();
        if (spawner != null)
        {
            spawner.EnemyDestroyed();
        }
        else
        {
            Debug.LogWarning("[Phasatron3] OnDestroy: no se encontró el Phasatron3Spawner para notificar derrota del enemigo.", this);
        }
    }

}
