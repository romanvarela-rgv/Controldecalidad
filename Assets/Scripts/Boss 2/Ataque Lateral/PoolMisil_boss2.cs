using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolMisil_boss2 : MonoBehaviour
{
    public GameObject MisilPrefab; // El prefab del misil
    public GameObject WarningPrefab; // Prefab para la advertencia visual
    public int poolSize = 20; // Cantidad de misiles en el pool
    private List<GameObject> pool;

    public float warningDuration = 1f; // Duración de la advertencia antes de disparar
    public float initialCooldown = 4f; // Cooldown inicial antes de disparar
    private bool canFire = false; // Controla si se puede disparar

    void Start()
    {
        // Inicializar el pool de misiles
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject proj = Instantiate(MisilPrefab);
            proj.SetActive(false); // Asegurarse de que los misiles no estén activos al inicio
            pool.Add(proj);
        }

        // Desactivar el WarningPrefab inicialmente
        if (WarningPrefab != null)
        {
            WarningPrefab.SetActive(false);
        }

        // Iniciar el cooldown inicial
        StartCoroutine(InitialCooldown());
    }

    public void FireMissileWithWarning(Vector3 position)
    {
        if (canFire)
        {
            StartCoroutine(ShowWarningAndFire(position));
        }
        else
        {
            Debug.Log("No se puede disparar aún. Cooldown en progreso.");
        }
    }

    private IEnumerator ShowWarningAndFire(Vector3 position)
    {
        // Mostrar advertencia activando el objeto WarningPrefab
        if (WarningPrefab != null)
        {
            WarningPrefab.transform.position = position; // Mover la advertencia a la posición especificada
            WarningPrefab.SetActive(true); // Activar la advertencia
        }

        yield return new WaitForSeconds(warningDuration); // Esperar el tiempo de la advertencia

        // Desactivar la advertencia después de la duración
        if (WarningPrefab != null)
        {
            WarningPrefab.SetActive(false);
        }

        // Disparar misil
        GameObject missile = GetProjectile();
        missile.transform.position = position;
        missile.transform.rotation = Quaternion.identity;
    }

    public GameObject GetProjectile()
    {
        foreach (var proj in pool)
        {
            if (!proj.activeInHierarchy) // Si el proyectil está inactivo
            {
                proj.SetActive(true); // Activarlo antes de usarlo
                return proj;
            }
        }

        // Si no hay proyectiles disponibles, crea uno nuevo (opcional)
        GameObject newProj = Instantiate(MisilPrefab);
        newProj.SetActive(true);
        pool.Add(newProj);
        return newProj;
    }

    public void ReturnProjectile(GameObject projectile)
    {
        projectile.SetActive(false); // Desactivar el objeto antes de devolverlo
    }

    private IEnumerator InitialCooldown()
    {
        // Esperar el cooldown inicial
        yield return new WaitForSeconds(initialCooldown);
        canFire = true; // Permitir disparar después del cooldown
    }
}
