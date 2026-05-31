using System.Collections.Generic;
using UnityEngine;

public class PoolProyectilesBoss2 : MonoBehaviour
{
    public static PoolProyectilesBoss2 Instance;

    [Header("Prefab & Pool")]
    [SerializeField] private GameObject projectilePrefab;  
    [SerializeField] private int initialSize = 20;         
    [SerializeField] private bool canExpand = true;       
    [SerializeField] private int expandBy = 10;           

    [Header("Reset al devolver")]
    [SerializeField] private bool resetPhysicsOnReturn = true;
    [SerializeField] private bool resetTransformOnReturn = true;

    private readonly List<GameObject> all = new();         
    private readonly Queue<GameObject> available = new();  
    private void Awake()
    {
        Instance = this;
        Prewarm(initialSize);
    }

    private void OnValidate()
    {
        if (initialSize < 0) initialSize = 0;
        if (expandBy < 1) expandBy = 1;
    }

    private void Prewarm(int count)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("[PoolProyectilesBoss2] Asigná 'projectilePrefab' en el inspector.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(projectilePrefab, transform); 
            go.SetActive(false);

            var comp = go.GetComponent<Boss_2_Proyectile>();
            if (comp != null) comp.SetPool(this);

            all.Add(go);
            available.Enqueue(go);
        }
    }

    public GameObject GetProjectile()
    {
        if (available.Count > 0)
        {
            var go = available.Dequeue();
            go.SetActive(true);
            return go;
        }

        if (canExpand)
        {
            Prewarm(expandBy);
            if (available.Count > 0)
            {
                var go = available.Dequeue();
                go.SetActive(true);
                return go;
            }
        }

        Debug.LogWarning("[PoolProyectilesBoss2] Pool agotado y no puede expandirse.");
        return null;
    }

    public void ReturnProjectile(GameObject projectile)
    {
        if (projectile == null) return;

        projectile.transform.SetParent(transform, worldPositionStays: false);

        if (resetPhysicsOnReturn)
        {
            var rb2d = projectile.GetComponent<Rigidbody2D>();
            if (rb2d != null)
            {
                rb2d.velocity = Vector2.zero;
                rb2d.angularVelocity = 0f;
            }

            var rb3d = projectile.GetComponent<Rigidbody>();
            if (rb3d != null)
            {
                rb3d.velocity = Vector3.zero;
                rb3d.angularVelocity = Vector3.zero;
            }
        }

        if (resetTransformOnReturn)
        {
            projectile.transform.localPosition = Vector3.zero;
            projectile.transform.localRotation = Quaternion.identity;
            projectile.transform.localScale = Vector3.one;
        }

        projectile.SetActive(false);
        available.Enqueue(projectile);
    }

    public void ClearPool()
    {
        for (int i = 0; i < all.Count; i++)
        {
            var go = all[i];
            if (go != null && go.activeInHierarchy)
                ReturnProjectile(go);
        }
    }

    public Boss_2_Proyectile Spawn(Vector3 position, Quaternion rotation, Vector3 direction, float lifetimeOverride = -1f)
    {
        var go = GetProjectile();
        if (go == null) return null;

        go.transform.SetPositionAndRotation(position, rotation);

        var comp = go.GetComponent<Boss_2_Proyectile>();
        if (comp != null)
            comp.SetDirection(direction, this, lifetimeOverride);

        return comp;
    }

    public int TotalCount => all.Count;
    public int AvailableCount => available.Count;
}
