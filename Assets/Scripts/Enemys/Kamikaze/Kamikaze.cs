using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

public class Kamikaze : MonoBehaviour
{
    [SerializeField] private HealthbarKamikaze healthbar;  // Referencia al script de la barra de salud
    [SerializeField] public float maxHealth = 100f;
    private float currentHealth;
    private SpriteRenderer spriteRenderer;

    public int valueKamikaze = 20;

    [SerializeField] private float dropProbability; // Probabilidad de que el enemigo suelte un objeto (30% en este caso)
    [SerializeField] private GameObject dropItemPrefab; // Prefab del objeto que puede soltar

    [SerializeField] private GameObject deattheffect;

    public AudioSource Clip;

    [SerializeField] private Color deathEffectColor = Color.white; // Color del efecto de muerte asignable desde el Inspector

    private void Start()
    {
        // Inicializar la salud y la barra de salud
        currentHealth = maxHealth;
        healthbar.UpdateHealthbar(maxHealth, currentHealth);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Aquí puedes agregar lógica adicional si es necesario
    }

    public void TakeDamage(float damage)
    {
        StartCoroutine(DamageCoroutine(damage));
    }

    private IEnumerator DamageCoroutine(float damage)
    {
        float damageDuration = 0.1f;
        currentHealth -= damage;
        healthbar.UpdateHealthbar(maxHealth, currentHealth);

        if (currentHealth > 0)
        {
            Clip.Play(); // Sonido de daño
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(damageDuration);
            spriteRenderer.color = Color.white;
        }
        else
        {
            TryDropItem();
            GameObject deathEffectInstance = Instantiate(deattheffect, transform.position, Quaternion.identity);
            SpriteRenderer deathSprite = deathEffectInstance.GetComponent<SpriteRenderer>();
            if (deathSprite != null)
            {
                deathSprite.color = deathEffectColor;
            }

            Clip.Play(); // Reproducir sonido de muerte

            // Esperar a que termine el sonido antes de destruir el objeto
            Destroy(gameObject, Clip.clip.length);

            GameManager.Instance.AddScore(valueKamikaze);
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
}
