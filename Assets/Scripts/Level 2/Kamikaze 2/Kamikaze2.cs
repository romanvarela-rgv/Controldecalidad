using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamikaze2 : MonoBehaviour
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

    [Header("Color Settings")]
    [SerializeField] private Color deathEffectColor = Color.white; // Color configurable del efecto de muerte desde el Inspector

    private void Start()
    {
        // Inicializar la salud y la barra de salud
        currentHealth = maxHealth;
        healthbar.UpdateHealthbar(maxHealth, currentHealth);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Aquí iría el código para actualizar el comportamiento del enemigo si es necesario
    }

    public void TakeDamage(float damage)
    {
        StartCoroutine(DamageCoroutine(damage));
    }

    private IEnumerator DamageCoroutine(float damage)
    {
        float damageDuration = 0.1f;
        currentHealth -= damage;  // Reducir la salud
        // Actualizar la barra de salud
        healthbar.UpdateHealthbar(maxHealth, currentHealth);

        if (currentHealth > 0)
        {
            Clip.Play();  // Reproducir sonido de daño

            spriteRenderer.color = Color.red;  // Efecto visual de daño
            yield return new WaitForSeconds(damageDuration);

            // Cambiar de vuelta al color original (puedes cambiar este color según el color original que deseas)
            Color normalColor;
            if (ColorUtility.TryParseHtmlString("#D39086", out normalColor))
            {
                spriteRenderer.color = normalColor;  // Volver al color especificado
            }
        }
        else
        {
            // Instanciar el efecto de muerte
            GameObject deathEffectInstance = Instantiate(deattheffect, transform.position, Quaternion.identity);

            // Si el sprite del efecto de muerte tiene un SpriteRenderer, podemos ajustar su color
            SpriteRenderer deathSprite = deathEffectInstance.GetComponent<SpriteRenderer>();
            if (deathSprite != null)
            {
                deathSprite.color = deathEffectColor;  // Usar el color configurado en el Inspector
            }

            // Intentar soltar el ítem (si aplica)
            TryDropItem();

            Clip.Play(); // Reproducir sonido de muerte

            // Destruir el enemigo
            Destroy(gameObject, Clip.clip.length);

            // Añadir puntuación
            GameManager.Instance.AddScore(valueKamikaze);

        }

        yield return null;  // Asegurarse de que la corutina termine correctamente
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
