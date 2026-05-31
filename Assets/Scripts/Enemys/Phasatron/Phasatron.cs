using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class Phasatron : MonoBehaviour
{
    [SerializeField] private HealthbarPhasatron healthbar;
    [SerializeField] public float maxHealth;
    [SerializeField] public float speed;
    [SerializeField] public float startWaitTime;

    [SerializeField] private float dropProbability;
    [SerializeField] private GameObject dropItemPrefab;
    [SerializeField] private float dropProbability2;
    [SerializeField] private GameObject dropItemPrefab2;
    [SerializeField] private GameObject deattheffect;
    [SerializeField] private Color deathEffectColor = Color.white;

    public Transform[] moveSpots;
    public GameObject EnemyGun;
    public int valuePhasatron = 40;
    public AudioSource Clip;

    private float health;
    private int randomSpot;
    private float waitTime;
    private SpriteRenderer spriteRenderer;
    private bool isDying;

    void Awake()
    {
        // Intentar auto-asignar componentes si faltan en el Inspector
        if (!spriteRenderer) TryGetComponent(out spriteRenderer);
        if (!Clip) TryGetComponent(out Clip);
        if (!healthbar) healthbar = GetComponentInChildren<HealthbarPhasatron>(true);

        if (!spriteRenderer) Debug.LogError("[Phasatron] Falta SpriteRenderer.", this);
        if (!Clip) Debug.LogWarning("[Phasatron] Falta AudioSource (Clip).", this);
        if (!healthbar) Debug.LogWarning("[Phasatron] Falta HealthbarPhasatron. La barra de vida no se actualizará.", this);
    }

    void Start()
    {
        health = maxHealth;
        if (healthbar) healthbar.UpdateHealthbar(maxHealth, health);

        waitTime = startWaitTime;
        randomSpot = Random.Range(0, moveSpots.Length);
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, moveSpots[randomSpot].position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, moveSpots[randomSpot].position) < 0.2f)
        {
            if (waitTime <= 0f)
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
        if (isDying) return;
        StartCoroutine(DamageCoroutine(damage));
    }

    private IEnumerator DamageCoroutine(float damage)
    {
        health -= damage;
        if (healthbar) healthbar.UpdateHealthbar(maxHealth, health);

        if (health > 0f)
        {
            if (Clip) Clip.Play();
            if (spriteRenderer)
            {
                spriteRenderer.color = Color.red;
                yield return new WaitForSeconds(0.1f);
                if (spriteRenderer) spriteRenderer.color = Color.white;
            }
            yield break;
        }

        // Muerte
        isDying = true;
        TryDropItem();

        // Instanciar el efecto de muerte con color configurable
        if (deattheffect)
        {
            GameObject deathEffectInstance = Instantiate(deattheffect, transform.position, Quaternion.identity);
            SpriteRenderer deathSprite = deathEffectInstance.GetComponent<SpriteRenderer>();
            if (deathSprite) deathSprite.color = deathEffectColor;
        }

        // Sumar puntos antes de destruir
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(valuePhasatron);
        }
        else
        {
            Debug.LogWarning("[Phasatron] GameManager.Instance es null al morir el Phasatron.", this);
        }

        Destroy(gameObject);
    }

    private void TryDropItem()
    {
        float randomValue = Random.value;

        // Primera probabilidad de drop
        if (randomValue <= dropProbability)
        {
            if (dropItemPrefab) Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
        }
        // Segunda probabilidad de drop (si es mayor que dropProbability)
        else if (randomValue <= dropProbability2)
        {
            if (dropItemPrefab2) Instantiate(dropItemPrefab2, transform.position, Quaternion.identity);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && GameManager.Instance != null)
        {
            GameManager.Instance.LoseLive(0.5f);
        }
    }

    private void OnDestroy()
    {
        // Evitar NullReference: solo notificar al spawner si existe
        PhasatronSpawner spawner = FindObjectOfType<PhasatronSpawner>();
        if (spawner != null)
        {
            spawner.EnemyDestroyed();
        }
    }
}
