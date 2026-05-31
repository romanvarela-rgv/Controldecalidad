using System.Collections;
using UnityEngine;

public class attackBoss : MonoBehaviour
{
    public GameObject missilePrefab; // Prefab del misil
    public Transform firePoint; // Punto de disparo
    public float missileSpeed = 5f; // Velocidad del misil
    public float rotationSpeed = 200f; // Velocidad de rotación del misil
    public float shootInterval = 2f; // Intervalo entre disparos

    private GameObject player; // Referencia al jugador

    private void Start()
    {
        // Buscar al jugador por etiqueta
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            // Comenzar el ciclo de disparo
            InvokeRepeating(nameof(ShootMissile), 2f, shootInterval);
        }
        else
        {
            Debug.LogError("No se encontró un objeto con la etiqueta 'Player'.");
        }
    }

    private void ShootMissile()
    {
        if (player != null && firePoint != null)
        {
            Debug.DrawLine(firePoint.position, player.transform.position, Color.red, 1f);
            GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
            MissileBehavior missileScript = missile.AddComponent<MissileBehavior>();
            missileScript.Initialize(player.transform, missileSpeed, rotationSpeed);
        }
        else
        {
            Debug.LogError("No se puede disparar: el jugador o firePoint son nulos.");
        }
    }
}
