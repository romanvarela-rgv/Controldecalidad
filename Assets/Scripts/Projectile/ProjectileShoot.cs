using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Necesario para usar el Slider

public class ProjectileShoot : MonoBehaviour
{
    public GameObject projectilePrefab;
    public PoolBulletPlayer bulletPool;
    public GameObject chargedShotPrefab; // Prefab para el disparo cargado
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float damage = 20f;
    public float lifespan;

    public Animator playerAnimator;
    public Slider chargedShotSlider; // Slider para el disparo cargado
    public float maxCharge=100;
    private float currentCharge = 0f;
    [SerializeField] public float chargePerHit = 20f;

    public Slider tripleShotSlider; // Slider para el disparo triple
    private bool isTripleShotActive = false;
    private float tripleShotDuration = 7f;
    private float tripleShotTimeLeft;

    [Header("Misiles Guiados")]
    public GameObject homingMissilePrefab;
    public Slider homingSlider;
    private bool isHomingActive = false;
    private float homingDuration = 7f;
    private float homingTimeLeft;

    [Header("Rapid Fire")]
    public Slider rapidFireSlider;
    [SerializeField] private float rapidFireDuration = 5f;
    [SerializeField] private float rapidFireCooldown = 0.08f;
    private bool isRapidFireActive = false;
    private float rapidFireTimeLeft;
    private float originalShootCooldown;

    public bool IsTripleShotActive => isTripleShotActive;

    public float shootCooldown = 0.5f; // Tiempo de cooldown entre disparos
    private float timeSinceLastShot;

    public AudioSource Clip;

    public GameObject warningImage; // Imagen de advertencia antes del disparo cargado
    public KeyCode chargedShotKey = KeyCode.F; // Tecla para disparar el cargado
    public float warningDuration = 1f;


    public AudioClip chargedShotSound;
    private AudioSource audioSource;
    public GameObject reinforcementsPrefab; // Prefab de las naves
    public Transform[] reinforcementPoints;
    public float spawnYPosition = -5f; // Posici�n Y desde donde las naves aparecer�n
    public float movementSpeed = 5f; // Velocidad de movimiento de las naves hacia el centro
    public float fireInterval = 0.5f; // Intervalo entre disparos de las naves
    public float shootingAngle = 0f; // �ngulo en el que las naves disparar�n

    [SerializeField] public float shootingDuration = 2f;
    [SerializeField] public float enterSpeed = 5f;

    [SerializeField] private float reinforcementLifetime = 5f;

    private bool canShoot = true; // Nueva variable de control

    void Start()
    {
        // Configuraci�n inicial de sliders
        tripleShotSlider.gameObject.SetActive(false);
        homingSlider.gameObject.SetActive(false);
        rapidFireSlider.gameObject.SetActive(false);
        timeSinceLastShot = shootCooldown;
        originalShootCooldown = shootCooldown;
        audioSource = GetComponent<AudioSource>();

        if (warningImage != null)
        {
            warningImage.SetActive(false); // Asegurar que inicia desactivada
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        timeSinceLastShot += Time.deltaTime;

        if (Input.GetButtonDown("Fire1") && timeSinceLastShot >= shootCooldown)
        {
            Shoot();
            timeSinceLastShot = 0f;
        }

        if (Input.GetButtonDown("Fire2") && currentCharge >= maxCharge && !isTripleShotActive && canShoot)
        {
            StartCoroutine(ShowWarningAndShoot());
            canShoot = false; // Bloquea nuevos disparos hasta que el cooldown termine
        }

        if (isTripleShotActive)
        {
            tripleShotTimeLeft -= Time.deltaTime;
            tripleShotSlider.value = tripleShotTimeLeft;

            if (tripleShotTimeLeft <= 0)
            {
                isTripleShotActive = false;
                tripleShotSlider.gameObject.SetActive(false);
            }
        }

        if (isHomingActive)
        {
            homingTimeLeft -= Time.deltaTime;
            homingSlider.value = homingTimeLeft;

            if (homingTimeLeft <= 0)
            {
                isHomingActive = false;
                homingSlider.gameObject.SetActive(false);
            }
        }

        if (isRapidFireActive)
        {
            rapidFireTimeLeft -= Time.deltaTime;
            rapidFireSlider.value = rapidFireTimeLeft;

            if (rapidFireTimeLeft <= 0)
            {
                isRapidFireActive = false;
                rapidFireSlider.gameObject.SetActive(false);
                shootCooldown = originalShootCooldown;
            }
        }
    }

    private IEnumerator ShowWarningAndShoot()
    {
        if (warningImage != null)
        {
            warningImage.SetActive(true);
        }

        yield return new WaitForSeconds(warningDuration); // Esperar

        if (warningImage != null)
        {
            warningImage.SetActive(false);
        }

        FireChargedShot();
        currentCharge = 0f;
        chargedShotSlider.value = currentCharge;

        yield return new WaitForSeconds(shootCooldown); // Esperar el cooldown antes de permitir otro disparo
        canShoot = true; // Permitir disparar de nuevo
    }

    public void ActivateTripleShot()
    {
        tripleShotTimeLeft = tripleShotDuration;
        tripleShotSlider.maxValue = tripleShotDuration;
        tripleShotSlider.value = tripleShotDuration;
        tripleShotSlider.gameObject.SetActive(true);
        isTripleShotActive = true;
    }

    void Shoot()
    {
        if (isHomingActive)
        {
            FireHomingMissile();
        }
        else if (isTripleShotActive)
        {
            FireProjectile(Vector2.up);
            FireProjectile(Quaternion.Euler(0, 0, 20) * Vector2.up);
            FireProjectile(Quaternion.Euler(0, 0, -20) * Vector2.up);
        }
        else
        {
            FireProjectile(Vector2.up);
        }
    }

    private void FireHomingMissile()
    {
        if (homingMissilePrefab == null) return;
        Instantiate(homingMissilePrefab, firePoint.position, firePoint.rotation);
    }

    public void ActivateHomingMissiles()
    {
        homingTimeLeft = homingDuration;
        homingSlider.maxValue = homingDuration;
        homingSlider.value = homingDuration;
        homingSlider.gameObject.SetActive(true);
        isHomingActive = true;
    }

    public void ActivateRapidFire()
    {
        rapidFireTimeLeft = rapidFireDuration;
        rapidFireSlider.maxValue = rapidFireDuration;
        rapidFireSlider.value = rapidFireDuration;
        rapidFireSlider.gameObject.SetActive(true);
        isRapidFireActive = true;
        shootCooldown = rapidFireCooldown;
    }

    private void FireProjectile(Vector2 direction)
    {
        GameObject bullet = bulletPool.GetBullet(lifespan);

        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = direction * bulletSpeed;
        }
    }

    private void FireChargedShot()
    { 
        audioSource.PlayOneShot(chargedShotSound);
        playerAnimator.SetTrigger("ChargedShot"); 
    
        InvokeReinforcements();
    }



    public void ForceFullCharge()
    {
        currentCharge = maxCharge;

        if (chargedShotSlider != null)
        {
            chargedShotSlider.maxValue = maxCharge;
            chargedShotSlider.value = currentCharge;
        }
    }

    public void ResetCharge()
    {
        currentCharge = 0f;

        if (chargedShotSlider != null)
        {
            chargedShotSlider.maxValue = maxCharge;
            chargedShotSlider.value = currentCharge;
        }
    }



    void InvokeReinforcements()
    {
        List<GameObject> spawnedReinforcements = new List<GameObject>(); // Lista para almacenar las naves creadas

        // Iterar por los puntos de aparici�n y crear naves
        foreach (Transform point in reinforcementPoints)
        {
            // Instanciar la nave en la posici�n del punto
            GameObject reinforcement = Instantiate(reinforcementsPrefab, point.position, Quaternion.identity);

            // Ajustar la rotaci�n seg�n el punto de aparici�n
            if (point.position.x < 0) // Si el punto est� a la izquierda
            {
                reinforcement.transform.rotation = Quaternion.Euler(0, 0, -90f); // Mirando hacia la derecha
            }
            else if (point.position.x > 0) // Si el punto est� a la derecha
            {
                reinforcement.transform.rotation = Quaternion.Euler(0, 0, 90f); // Mirando hacia la izquierda
            }

            // Agregar la nave a la lista
            spawnedReinforcements.Add(reinforcement);

            // Hacer que las naves disparen desde su posici�n
            StartCoroutine(ShootAtCenter(reinforcement));
        }

        // Iniciar la corrutina para destruirlas despu�s de reinforcementLifetime segundos
        StartCoroutine(DestroyReinforcements(spawnedReinforcements));
    }

    // Corrutina para destruir las naves despu�s de reinforcementLifetime segundos
    private IEnumerator DestroyReinforcements(List<GameObject> reinforcements)
    {
        yield return new WaitForSeconds(reinforcementLifetime);

        foreach (GameObject reinforcement in reinforcements)
        {
            if (reinforcement != null)
            {
                Destroy(reinforcement); // Eliminar la nave de la escena
            }
        }
    }

    private IEnumerator MoveReinforcementIn(GameObject reinforcement, bool fromLeft)
    {
        // Definir la posici�n final (en el centro en el eje X)
        Vector3 targetPosition = new Vector3(0f, spawnYPosition, 0f); // Centro en X
        float startX = fromLeft ? -10f : 10f; // Si viene de la izquierda o de la derecha
        reinforcement.transform.position = new Vector3(startX, spawnYPosition, 0f);

        // Mover la nave hacia el centro
        while (Mathf.Abs(reinforcement.transform.position.x - targetPosition.x) > 0.1f)
        {
            float moveDirection = fromLeft ? 1f : -1f; // Si la nave viene de la izquierda o de la derecha
            reinforcement.transform.position += new Vector3(moveDirection * enterSpeed * Time.deltaTime, 0, 0);
            yield return null;
        }

        // Una vez que la nave entra, se queda en su posici�n
        reinforcement.transform.position = targetPosition;
    }

    private IEnumerator ShootAtCenter(GameObject reinforcement)
    {
        // Esperar antes de empezar a disparar (para dar tiempo a que la nave entre completamente)
        yield return new WaitForSeconds(1f);

        float timeElapsed = 0f;
        // Determinar la direcci�n seg�n la posici�n de la nave
        Vector2 shootDirection = reinforcement.transform.position.x < 0 ? Vector2.right : Vector2.left;

        // Realizar disparos hacia el centro durante unos segundos
        while (timeElapsed < shootingDuration)
        {
            FireProjectileFromReinforcement(reinforcement); // Disparar hacia la direcci�n calculada
            timeElapsed += fireInterval;
            yield return new WaitForSeconds(fireInterval);
        }
    }

    private void FireProjectileFromReinforcement(GameObject reinforcement)
    {
        Transform firePoint = reinforcement.transform.Find("FirePoint");

        if (firePoint != null)
        {
            GameObject bullet = bulletPool.GetBullet(lifespan);

            if (bullet != null)
            {
                bullet.transform.position = firePoint.position;
                bullet.transform.rotation = Quaternion.identity; // Asegurar que no tenga rotaci�n

                // Asignar la direcci�n en funci�n de la posici�n del FirePoint
                Projectile2 projectileScript = bullet.GetComponent<Projectile2>();
                if (projectileScript != null)
                {
                    // Si el proyectil sale del lado derecho
                    if (firePoint.position.x > 0)
                    {
                        projectileScript.moveDirection = new Vector2(0, 0);  // Hacia la izquierda
                    }
                    // Si el proyectil sale del lado izquierdo
                    else
                    {
                        projectileScript.moveDirection = new Vector2(0, 0);  // Hacia la derecha
                    }
                }

                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.velocity = new Vector2(projectileScript.moveDirection.x * bulletSpeed, 0);  // Solo mover en X
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PowerUpShoot"))
        {
            Clip.Play();
            ActivateTripleShot();
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("PowerUpHoming"))
        {
            Clip.Play();
            ActivateHomingMissiles();
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("PowerUpRapidFire"))
        {
            Clip.Play();
            ActivateRapidFire();
            Destroy(collision.gameObject);
        }
    }

    public void IncreaseCharge()
    {
        currentCharge += chargePerHit;
        currentCharge = Mathf.Clamp(currentCharge, 0f, maxCharge);  // Limitar a 100 m�ximo
        chargedShotSlider.value = currentCharge;
    }
}
