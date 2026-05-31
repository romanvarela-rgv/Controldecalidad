using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReinforcementShip : MonoBehaviour
{
    [Header("Disparo")]
    [SerializeField] private GameObject projectilePrefab; 
    [SerializeField] private Transform firePoint;        
    [SerializeField] private float bulletSpeed = 5f;      
    [SerializeField] private float fireInterval = 0.5f;  

    [Header("Límite (opcional)")]
    [Tooltip("0 = sin límite. Si > 0, deja de disparar al alcanzar este número.")]
    [SerializeField] private int maxShotsFromThisShip = 0;

    private bool isShooting = false;
    private Coroutine shootLoop;
    private WaitForSeconds wait;

    private readonly HashSet<GameObject> activeBullets = new HashSet<GameObject>();
    private int shotsFiredFromThisShip = 0;

    private void OnEnable()
    {
        wait = new WaitForSeconds(Mathf.Max(0.01f, fireInterval));

        shotsFiredFromThisShip = 0;
        isShooting = true;
        shootLoop = StartCoroutine(ShootAtCenter());
    }

    private void OnDisable()
    {
        StopShootingAndClear();
    }

    private void OnDestroy()
    {
        StopShootingAndClear();
    }

    private IEnumerator ShootAtCenter()
    {
        while (isShooting && enabled)
        {
            TryFireOnce();
            yield return wait;
        }
    }

    private void TryFireOnce()
    {
        if (!projectilePrefab || !firePoint) return;

        if (maxShotsFromThisShip > 0 && shotsFiredFromThisShip >= maxShotsFromThisShip)
        {
            StopShootingAndClear();
            return;
        }

        FireProjectile();
        shotsFiredFromThisShip++;
    }

    private void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        if (projectile.TryGetComponent<Rigidbody2D>(out var rb))
            rb.velocity = Vector2.up * bulletSpeed;

        activeBullets.Add(projectile);

    }

    public void StopShootingAndClear()
    {
        if (shootLoop != null)
        {
            StopCoroutine(shootLoop);
            shootLoop = null;
        }

        if (!isShooting)
        {
            CleanupBullets();
            return;
        }

        isShooting = false;
        CleanupBullets();
    }

    private void CleanupBullets()
    {
        if (activeBullets.Count > 0)
        {
            foreach (var b in activeBullets)
            {
                if (b != null) Destroy(b);
            }
            activeBullets.Clear();
        }
    }

    public void OnProjectileGone(GameObject bullet)
    {
        if (bullet != null) activeBullets.Remove(bullet);
    }
}
