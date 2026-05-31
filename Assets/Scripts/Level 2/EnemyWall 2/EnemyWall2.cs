using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EnemyWall2 : MonoBehaviour
{
    [SerializeField] private HealthbarEnemyWall healthbarPared;

    private SpriteRenderer spriteRenderer;
    [SerializeField] public float maxHealth;
    private float health;
    private float currentHealth;

    public int valuePared = 10;

    [Header("Target Follow")]
    [SerializeField] private Transform target;           // cache del objetivo
    [SerializeField] private string targetTag = "EnemyPhasatron";
    [SerializeField] private float reacquireInterval = 0.5f; // reintenta 2 veces/seg si se pierde
    private float nextSearchTime;

    public float moveSpeed = 3f;
    public float yDistance = 4f;

    [Header("Drops")]
    [SerializeField] private float dropProbability;
    [SerializeField] private GameObject dropItemPrefab;

    [SerializeField] private float dropProbability2;
    [SerializeField] private GameObject dropItemPrefab2;

    [SerializeField] private GameObject deattheffect;
    public AudioSource Clip;

    public Color enemyColor = Color.white;
    [SerializeField] private Color deathEffectColor = Color.white;

    // Cache para evitar GC en parpadeo de daño
    private static readonly WaitForSeconds hitFlashDelay = new WaitForSeconds(0.1f);

    void Awake()
    {
        // Autowire defensivo
        if (!spriteRenderer) TryGetComponent(out spriteRenderer);
        if (!Clip) TryGetComponent(out Clip);
        if (!healthbarPared) healthbarPared = GetComponentInChildren<HealthbarEnemyWall>(true); // incluye inactivos

        if (!spriteRenderer) Debug.LogError("[EnemyWall2] Falta SpriteRenderer.", this);
        if (!Clip) Debug.LogWarning("[EnemyWall2] Falta AudioSource (Clip).", this);
        if (!healthbarPared) Debug.LogWarning("[EnemyWall2] Falta HealthbarEnemyWall. No se actualizará la UI.", this);
    }

    void OnEnable()
    {
        TryAcquireTarget(); // primer intento
    }

    void Start()
    {
        health = maxHealth;
        if (healthbarPared) healthbarPared.UpdateHealthbar(maxHealth, health);
        // (target pudo haber quedado seteado en OnEnable; si no, se reintentará en Update)
    }

    void Update()
    {
        // mover solo si hay objetivo válido
        if (target != null)
        {
            FollowTargetWithYDistance();
        }
        else if (Time.time >= nextSearchTime)
        {
            TryAcquireTarget(); // reintento espaciado (no cada frame)
        }
    }

    private void TryAcquireTarget()
    {
        // Devuelve null si no hay GO activo con ese tag
        var go = GameObject.FindWithTag(targetTag);
        if (go != null) target = go.transform;
        nextSearchTime = Time.time + reacquireInterval;
    }

    private void FollowTargetWithYDistance()
    {
        // Seguridad extra si el objetivo fue destruido entre frames
        if (target == null) return;

        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = target.position;

        if (Mathf.Abs(currentPosition.y - targetPosition.y) > yDistance)
        {
            currentPosition.y = Mathf.MoveTowards(currentPosition.y, targetPosition.y, moveSpeed * Time.deltaTime);
        }

        currentPosition.x = Mathf.MoveTowards(currentPosition.x, targetPosition.x, moveSpeed * Time.deltaTime);
        transform.position = currentPosition;
    }

    public void TakeDamage(float damage)
    {
        StartCoroutine(DamageCoroutine(damage));
    }

    private IEnumerator DamageCoroutine(float damage)
    {
        health -= damage;
        if (healthbarPared) healthbarPared.UpdateHealthbar(maxHealth, health);

        if (health > 0f)
        {
            if (Clip) Clip.Play();
            if (spriteRenderer)
            {
                spriteRenderer.color = Color.red;
                yield return hitFlashDelay;
                // Volver a color definido (si falla parse, poner blanco)
                if (ColorUtility.TryParseHtmlString("#FFB6B6", out Color normalColor))
                    spriteRenderer.color = normalColor;
                else
                    spriteRenderer.color = Color.white;
            }
            yield break;
        }

        // Muerte
        TryDropItem();
        TryDropItem2();

        if (deattheffect)
        {
            GameObject deathEffectInstance = Instantiate(deattheffect, transform.position, Quaternion.identity);
            var deathSprite = deathEffectInstance.GetComponent<SpriteRenderer>();
            if (deathSprite) deathSprite.color = deathEffectColor;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(valuePared);
        }
        else
        {
            Debug.LogWarning("[EnemyWall2] GameManager.Instance es null al morir EnemyWall2.", this);
        }

        Destroy(gameObject);
        yield return null;
    }

    private void TryDropItem()
    {
        if (dropItemPrefab && Random.value <= dropProbability)
            Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
    }

    private void TryDropItem2()
    {
        if (dropItemPrefab2 && Random.value <= dropProbability2)
            Instantiate(dropItemPrefab2, transform.position, Quaternion.identity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject && collision.gameObject.CompareTag("Player")) // CompareTag recomendado
        {
            if (GameManager.Instance != null) GameManager.Instance.LoseLive(1f);
            else Debug.LogWarning("[EnemyWall2] GameManager.Instance es null en OnCollisionEnter2D.", this);
        }
    }

    private void OnDestroy()
    {
        // El spawner puede no existir (o ser destruido primero) durante el teardown
        var spawner = FindObjectOfType<EnemyWallSpawner>();
        if (spawner != null)
            spawner.EnemyWallDestroyed();
        // else: sin warning para no ensuciar teardown, pero si querés: Debug.LogWarning(...)
    }
}
