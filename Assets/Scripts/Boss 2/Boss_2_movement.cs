using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class Boss_2_movement : MonoBehaviour
{
    private float moveSpeed;
    private bool moveRight;
    private float waitTime;

    [SerializeField] private HealthbarBoss healthbar;
    [SerializeField] public float maxHealth;

    private SpriteRenderer spriteRenderer;

    private float health;
    private float currentHealth;
    public int valueBoss = 200;

    public float speed;
    public float startWaitTime;

    public Transform[] MoveBoss;
    public int targetPoint;
    private int randomSpot;

    public AudioSource Clip;

    // Optimiz.: cache de yield para no alocar en cada hit
    private static readonly WaitForSeconds hitFlashDelay = new WaitForSeconds(0.1f);

    private bool isDying;

    void Awake()
    {
        // Autowire defensivo — evita NRE si olvidaste arrastrar referencias
        if (!spriteRenderer) TryGetComponent(out spriteRenderer);
        if (!Clip) TryGetComponent(out Clip);
        if (!healthbar) healthbar = GetComponentInChildren<HealthbarBoss>(true);

        if (!spriteRenderer) Debug.LogError("[Boss_2_movement] Falta SpriteRenderer.", this);
        if (!Clip) Debug.LogWarning("[Boss_2_movement] Falta AudioSource (Clip). No sonará al recibir daño.", this);
        if (!healthbar) Debug.LogWarning("[Boss_2_movement] Falta HealthbarBoss. No se actualizará la UI.", this);
    }

    void Start()
    {
        health = maxHealth;

        // UI de vida (si existe)
        if (healthbar) healthbar.UpdateHealthbar(maxHealth, health);

        targetPoint = 0;
        waitTime = startWaitTime;

        // Validación de waypoints
        if (MoveBoss == null || MoveBoss.Length == 0)
        {
            enabled = false;
            return;
        }

        randomSpot = Random.Range(0, MoveBoss.Length);
    }

    void Update()
    {
        // Si quedó deshabilitado por falta de waypoints no seguir
        if (!enabled) return;

        // Seguridad adicional: si el spot actual es nulo, elige otro válido
        if (MoveBoss[randomSpot] == null)
        {
            // Buscar el primer waypoint no nulo
            int fallback = System.Array.FindIndex(MoveBoss, t => t != null);
            if (fallback < 0)
            {
                enabled = false;
                return;
            }
            randomSpot = fallback;
        }

        // Movimiento hacia el waypoint actual
        transform.position = Vector2.MoveTowards(
            transform.position,
            MoveBoss[randomSpot].position,
            speed * Time.deltaTime
        );

        // Llegada al punto
        if (Vector2.Distance(transform.position, MoveBoss[randomSpot].position) < 0.2f)
        {
            if (waitTime <= 0f)
            {
                // Elegir nuevo punto válido
                int tries = 0;
                do
                {
                    randomSpot = Random.Range(0, MoveBoss.Length);
                    tries++;
                } while (MoveBoss[randomSpot] == null && tries < MoveBoss.Length);

                if (MoveBoss[randomSpot] == null)
                {
                    enabled = false;
                    return;
                }

                waitTime = startWaitTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.LoseLive(0.5f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (!enabled) return;   // si el script fue deshabilitado por falta de refs
        if (isDying) return;    // evita reentradas al morir
        StartCoroutine(DamageCoroutine(damage));
    }

    private IEnumerator DamageCoroutine(float damage)
    {
        // Reducir salud y actualizar UI si existe
        health -= damage;
        if (healthbar) healthbar.UpdateHealthbar(maxHealth, health);

        if (health > 0f)
        {
            if (Clip) Clip.Play();
            if (spriteRenderer)
            {
                spriteRenderer.color = Color.red;
                yield return hitFlashDelay; // cacheado (evita GC)
                if (spriteRenderer) spriteRenderer.color = Color.white;
            }
            yield break;
        }

        isDying = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(valueBoss);
            GameManager.Instance.EndLevel(2);
        }
        Destroy(gameObject);

        var current = SceneManager.GetActiveScene();
        LevelProgressStore.SetLastCompletedName(current.name);

        SceneManager.LoadScene("WinScreen");
    }
}
