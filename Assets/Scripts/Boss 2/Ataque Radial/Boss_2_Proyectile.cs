using System.Collections;
using UnityEngine;

public class Boss_2_Proyectile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    [Header("Lifetime")]
    public float lifetime = 5f; 
    private Coroutine autoDisableRoutine;

    private Vector3 direction;                  
    private PoolProyectilesBoss2 pool;         

    private void OnEnable()
    {
        // timer de vida
        if (lifetime > 0f)
            autoDisableRoutine = StartCoroutine(AutoDisable(lifetime));
    }

    private void Update()
    {
        if (pool == null) return;

        transform.position += direction * speed * Time.deltaTime;

        // Auto-off si sale de pantalla
        var cam = Camera.main;
        if (cam != null)
        {
            Vector2 min = cam.ViewportToWorldPoint(new Vector2(0f, 0f));
            Vector2 max = cam.ViewportToWorldPoint(new Vector2(1f, 1f));
            Vector3 p = transform.position;

            if (p.x < min.x || p.x > max.x || p.y < min.y || p.y > max.y)
                ReturnToPool();
        }
    }

    private IEnumerator AutoDisable(float t)
    {
        yield return new WaitForSeconds(t);
        ReturnToPool();
    }

    private void OnDisable()
    {
        if (autoDisableRoutine != null)
        {
            StopCoroutine(autoDisableRoutine);
            autoDisableRoutine = null;
        }
    }

    public void SetPool(PoolProyectilesBoss2 proyectilePool)
    {
        pool = proyectilePool;
    }

    public void SetDirection(Vector3 newDirection, PoolProyectilesBoss2 poolReference, float lifetimeOverride = -1f)
    {
        direction = newDirection.sqrMagnitude > 0.0001f ? newDirection.normalized : Vector3.zero;
        pool = poolReference;
        if (lifetimeOverride >= 0f) lifetime = lifetimeOverride;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.LoseLive(1f);
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (pool != null) pool.ReturnProjectile(gameObject);
        else gameObject.SetActive(false);
    }
}
