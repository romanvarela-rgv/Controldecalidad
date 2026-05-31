using System.Collections;
using UnityEngine;

public class Rayo : MonoBehaviour
{
    private float damage = 1.5f;  // Daño que aplicará al colisionar
    private float life = 32f;
   [SerializeField] private float activationDelay ; // Tiempo de espera antes de activar la colisión

    private SpriteRenderer spriteRenderer;
    private Collider2D rayoCollider;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rayoCollider = GetComponent<Collider2D>();

        // Desactiva el collider al inicio
        rayoCollider.enabled = false;

        // Inicia la coroutine para activar el collider después del retraso
        StartCoroutine(ActivateColliderAfterDelay());
    }

    private IEnumerator ActivateColliderAfterDelay()
    {
        yield return new WaitForSeconds(activationDelay);
        rayoCollider.enabled = true; // Activa el collider después del retraso
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Llama al método de pérdida de vida en el GameManager
            GameManager.Instance.LoseLive(0.3f);

            // Llama al método de daño del jugador para ralentizarlo
            DashMovement player = other.GetComponent<DashMovement>();
            if (player != null)
            {
                player.TakeDamage(5); // Puedes ajustar el daño según lo que prefieras
            }

            // Desactiva el rayo después del impacto
            

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
            Destroy(gameObject);  // Destruir el rayo
        }
    }
}
