using System.Collections;
using UnityEngine;

public class MeteoritoSpawnerL : MonoBehaviour
{
    [SerializeField] private MeteoritoPool meteoritoPool;   // Asigna aquí tu pool de meteoritos
    [SerializeField] private float spawnInterval = 15f;
    [SerializeField] private float meteoritoSpeed = 5f;
    [SerializeField] private Vector2 spawnPosition = new Vector2(0f, 6f);
    [SerializeField] private float meteoritoLifetime = 10f; // Tiempo antes de devolver al pool

    [SerializeField] private GameObject warningSprite;
    [SerializeField] private float warningDuration = 3f;

    private void Awake()
    {
        if (warningSprite != null)
        {
            warningSprite.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (GameManager.Instance == null || !GameManager.Instance.TutorialCompleted) return;

        // Usamos nameof para evitar errores de cadena y activar el spawn repetido
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
            Debug.LogWarning("[MeteoritoSpawnerL] meteoritoPool es null. Asigna una referencia al pool en el Inspector.", this);
            return;
        }

        // Obtener un meteorito del pool y colocarlo en la posición de spawn
        GameObject meteorito = meteoritoPool.GetMeteorito();
        meteorito.transform.position = spawnPosition;

        // Mantener la misma dirección diagonal hacia la derecha (2f, -meteoritoSpeed)
        Rigidbody2D rb = meteorito.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(2f, -meteoritoSpeed);
        }

        // Devolver el meteorito al pool tras un tiempo de vida, si se desea
        if (meteoritoLifetime > 0f)
        {
            StartCoroutine(ReturnMeteoritoAfterTime(meteorito, meteoritoLifetime));
        }
    }

    private IEnumerator ReturnMeteoritoAfterTime(GameObject meteorito, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        // Comprobar que sigue activo antes de devolverlo
        if (meteorito != null && meteorito.activeInHierarchy)
        {
            meteoritoPool.ReturnMeteorito(meteorito);
        }
    }
}
