using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EnemyWall : MonoBehaviour
{
    [SerializeField] private HealthbarEnemyWall healthbarPared;

    private SpriteRenderer spriteRenderer;
    [SerializeField] public float maxHealth;
    private float health;
    private float currentHealth;

    public int valuePared = 10;

    [Header("Target Follow")]
    [SerializeField] private Transform target;                  // cache del objetivo
    [SerializeField] private string targetTag = "EnemyPhasatron";
    [SerializeField] private float reacquireInterval = 0.5f;    // reintenta 2 veces/seg si se pierde
    private float nextSearchTime;

    public float moveSpeed = 3f;
    public float yDistance = 4f;

    [Header("Drops")]
    [SerializeField] private float dropProbability;
    [SerializeField] private GameObject dropItemPrefab;

    [SerializeField] private float dropProbability2;
    [SerializeField] private GameObject dropItemPrefab2;

    [SerializeField] private GameObject deattheffect;
    public AudioClip impactSound;
    public AudioSource Clip;

    [SerializeField] private Color deathEffectColor = Color.white;

    // cache para evitar GC en el parpadeo de daño
    private static readonly WaitForSeconds hitFlashDelay = new WaitForSeconds(0.1f);

    void Awake()
    {
        // Autowire defensivo
        if (!spriteRenderer) TryGetComponent(out spriteRenderer);
        if (!Clip) TryGetComponent(out Clip);
        if (!healthbarPared) healthbarPared = GetComponentInChildren<HealthbarEnemyWall>(true);

        if (!spriteRenderer) Debug.LogError("[EnemyWall] Falta SpriteRenderer.", this);
        if (!Clip) Debug.LogWarning("[EnemyWall] Falta AudioSource (Clip).", this);
        if (!healthbarPared) Debug.LogWarning("[EnemyWall] Falta HealthbarEnemyWall. No se actualizará la UI.", this);
    }

    void OnEnable()
    {
        TryAcquireTarget(); // primer intento
    }

    void Start()
    {
        health = maxHealth;
        if (healthbarPared) healthbarPared.UpdateHealthbar(maxHealth, health);
        // si target no está, se reintenta en Update con intervalo
    }

    void Update()
    {
        if (target != null)
        {
            FollowTargetWithYDistance();
        }
        else if (Time.time >= nextSearchTime)
        {
            TryAcquireTarget();
        }
    }

    private void TryAcquireTarget()
    {
        // FindWithTag devuelve null si no hay activos con ese tag
        var go = GameObject.FindWithTag(targetTag);
        if (go != null) target = go.transform;
        nextSearchTime = Time.time + reacquireInterval;
    }

    private void FollowTargetWithYDistance()
    {
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
                spriteRenderer.color = Color.white; // color normal
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
            Debug.LogWarning("[EnemyWall] GameManager.Instance es null al morir EnemyWall.", this);
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
        if (collision.gameObject && collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance != null) GameManager.Instance.LoseLive(0.5f);
            else Debug.LogWarning("[EnemyWall] GameManager.Instance es null en OnCollisionEnter2D.", this);
        }
    }

    private void OnDestroy()
    {
        // El spawner puede no existir o destruirse antes durante teardown
        var spawner = FindObjectOfType<EnemyWallSpawner>();
        if (spawner != null)
            spawner.EnemyWallDestroyed();
    }
}
