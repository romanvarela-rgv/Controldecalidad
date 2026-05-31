using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBulletPlayer : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize = 40;
    [SerializeField] private Transform poolParent;

    private readonly Queue<GameObject> availableBullets = new Queue<GameObject>();
    private readonly Dictionary<GameObject, Coroutine> lifeCoroutines = new Dictionary<GameObject, Coroutine>();
    private readonly HashSet<GameObject> inUse = new HashSet<GameObject>();
    private bool isQuitting = false;

    private void Awake()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("[PoolBulletPlayer] Prefab de bala NO asignado.", this);
            enabled = false;
            return;
        }

        if (poolParent == null)
        {
            var go = new GameObject("BulletPool");
            poolParent = go.transform;
        }

        // Si querés que el pool sobreviva al cambio de escena, descomentá:
        // DontDestroyOnLoad(gameObject);

        for (int i = 0; i < poolSize; i++)
            CreateBullet();
    }

    private void OnApplicationQuit() => isQuitting = true;

    private GameObject CreateBullet()
    {
        var bullet = Instantiate(bulletPrefab, poolParent);
        bullet.SetActive(false);
        availableBullets.Enqueue(bullet);
        return bullet;
    }

    public GameObject GetBullet(float lifespan = 0f)
    {
        GameObject bullet = null;

        // Tomar una bala válida; si hay nulls (destruidas por escena), las saltamos
        while (availableBullets.Count > 0 && bullet == null)
        {
            bullet = availableBullets.Dequeue();
            if (bullet == null) { /* hueco corrupto */ continue; }
        }

        if (bullet == null)
            bullet = CreateBullet();

        // Cancelar timer viejo si quedaba alguno
        if (lifeCoroutines.TryGetValue(bullet, out var running) && running != null)
        {
            StopCoroutine(running);
            lifeCoroutines.Remove(bullet);
        }

        inUse.Add(bullet); // marcar “en uso” (impide doble Return)
        bullet.transform.SetParent(null, false);
        bullet.SetActive(true);

        if (lifespan > 0f)
        {
            var co = StartCoroutine(DeactivateAfterTime(bullet, lifespan));
            lifeCoroutines[bullet] = co;
        }

        return bullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        if (bullet == null || isQuitting) return;

        // Evitar doble retorno / encolado duplicado
        if (!inUse.Remove(bullet)) return;

        // Cancelar el timer si existía
        if (lifeCoroutines.TryGetValue(bullet, out var running) && running != null)
        {
            StopCoroutine(running);
        }
        lifeCoroutines.Remove(bullet);

        // Guardas de seguridad (puede haber sido desactivada por otro script)
        if (!bullet) return;

        bullet.SetActive(false);
        bullet.transform.SetParent(poolParent, false);
        availableBullets.Enqueue(bullet);
    }

    private IEnumerator DeactivateAfterTime(GameObject bullet, float time)
    {
        yield return new WaitForSeconds(time);

        // La bala puede haberse destruido o devuelto por colisión/escena
        if (!bullet) yield break;

        // Sólo devolver si sigue activa y sigue “en uso”
        if (bullet.activeInHierarchy && inUse.Contains(bullet))
            ReturnBullet(bullet);
    }
}
