using System.Collections;
using UnityEngine;

public class MeteoritoSpawner : MonoBehaviour
{
    [SerializeField] private MeteoritoPool meteoritoPool; // Referencia al pool
    [SerializeField] private float spawnInterval = 15f;
    [SerializeField] private float meteoritoSpeed = 5f;
    [SerializeField] private Vector2 spawnPosition = new Vector2(0f, 6f);
    [SerializeField] private float meteoritoLifetime = 10f; // Tiempo antes de devolver al pool

    [SerializeField] private GameObject warningSprite;
    [SerializeField] private float warningDuration = 3f;

    private float timer;


    private void Awake()
    {
        if (warningSprite != null)
        {
            warningSprite.SetActive(false);
        }
    }

    private void OnEnable()
    {
        timer = spawnInterval;

        InvokeRepeating(nameof(StartWarningSequence), spawnInterval, spawnInterval);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(StartWarningSequence));
    }

    private void StartWarningSequence()
    {
        if (warningSprite != null)
        {
            StartCoroutine(WarningSequence());
        }
    }

    private IEnumerator WarningSequence()
    {
        warningSprite.SetActive(true);
        yield return new WaitForSeconds(warningDuration);
        warningSprite.SetActive(false);
        SpawnMeteorito();
    }

    private void SpawnMeteorito()
    {
        if (meteoritoPool == null)
        {
            Debug.LogWarning("[MeteoritoSpawner] meteoritoPool es null. Asigna una referencia al pool.", this);
            return;
        }
        GameObject meteorito = meteoritoPool.GetMeteorito();
        meteorito.transform.position = spawnPosition;
        Rigidbody2D rb = meteorito.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(-2f, -meteoritoSpeed);
        }
        if (meteoritoLifetime > 0f)
        {
            StartCoroutine(ReturnMeteoritoAfterTime(meteorito, meteoritoLifetime));
        }
    }

    private IEnumerator ReturnMeteoritoAfterTime(GameObject meteorito, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        if (meteorito != null && meteorito.activeInHierarchy)
        {
            meteoritoPool.ReturnMeteorito(meteorito);
        }
    }
}
