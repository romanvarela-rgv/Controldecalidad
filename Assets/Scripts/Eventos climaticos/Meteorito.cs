using System.Collections;
using UnityEngine;

public class Meteorito : MonoBehaviour
{
    public float damage = 1.5f;  // Daño que aplicará al colisionar
    public float life = 32f;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.LoseLive();
            gameObject.SetActive(false);

            // Instancia aquí la animación de impacto si es necesario.
        }
    }

    public void TakeDamage(float damage)
    {
        StartCoroutine(DamageCoroutine(damage));
    }

    private IEnumerator DamageCoroutine(float damage)
    {
        float damageDuration = 0.1f;
        life -= damage;

        if (life > 0)
        {
            spriteRenderer.color = Color.red;  // Efecto visual de daño
            yield return new WaitForSeconds(damageDuration);
            spriteRenderer.color = Color.white;  // Volver al color original
        }
        else
        {
            Destroy(gameObject);  // Destruir el meteorito
        }
    }
}

