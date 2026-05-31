using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile1 : MonoBehaviour
{
    public GameManager gameManager;
    public float moveSpeed;
    public float damage;
    public float lifespan = 1f; // Duración de la bala antes de desactivarse
    private PoolBulletPlayer bulletPool;

    public AudioClip shootSound;          // Sonido al disparar
    public AudioClip impactSound;         // Sonido al impactar
    private AudioSource audioSource;      // Componente para reproducir sonido

    public ProjectileShoot projectileShoot;

    void OnEnable()
    {
        if (bulletPool == null)
        {
            bulletPool = FindObjectOfType<PoolBulletPlayer>();
        }
        if (projectileShoot == null)
        {
       
        }
        
        if (projectileShoot != null)
        {
            // Buscar al Player en la escena
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
              ;
                // Obtener el componente ProjectileShoot del Player
                projectileShoot = player.GetComponent<ProjectileShoot>();
                if (projectileShoot != null)
                {
                    
                }
                else
                {
                    
                }
            }
            else
            {
               
            }
        }

        audioSource = GetComponent<AudioSource>();

        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        StartCoroutine(DeactivateAfterTime(lifespan));
    }

    void Awake()
    {
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }


    void Update()
    {
        transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        // Comprobar colisiones con enemigos y aplicar daño
        if (collision.gameObject.CompareTag("EnemyPhasatron"))
        {
            Phasatron enemy = collision.gameObject.GetComponent<Phasatron>();
            Phasatron2 enemy2 = collision.gameObject.GetComponent<Phasatron2>();
            Phasatron3 enemy3 = collision.gameObject.GetComponent<Phasatron3>();
            if (enemy != null)
            {
                float damageAmount = enemy.maxHealth * 0.21f;
                enemy.TakeDamage(damageAmount);
                
            }
            if (enemy2 != null)
            {
                float damageAmount = enemy2.maxHealth * 0.16f;
                enemy2.TakeDamage(damageAmount);

            }
            if(enemy3 != null)
            {
                float damageAmount = enemy3.maxHealth * 0.13f;
                enemy3.TakeDamage(damageAmount);
            }


            DeactivateBullet();
        }

        if (collision.gameObject.CompareTag("EnemyWall"))
        {
            EnemyWall healthPared = collision.gameObject.GetComponent<EnemyWall>();
            EnemyWall2 healthPared2 = collision.gameObject.GetComponent<EnemyWall2>();
            if (healthPared != null)
            {
                float damageAmount = healthPared.maxHealth * 0.40f;
                healthPared.TakeDamage(damageAmount);
            }
            if (healthPared2 != null)
            {
                float damageAmount = healthPared2.maxHealth * 0.35f;
                healthPared2.TakeDamage(damageAmount);
            }

            DeactivateBullet();
        }

        if (collision.gameObject.CompareTag("EnemyKamikaze"))
        {
            Kamikaze enemyHealth = collision.GetComponent<Kamikaze>();
            Kamikaze2 enemyHealth2 = collision.GetComponent<Kamikaze2>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(60f);
            }
            if (enemyHealth2 != null)
            {
                enemyHealth2.TakeDamage(55f);
            }


            DeactivateBullet();
        }

        if (collision.gameObject.CompareTag("CrazyKamikaze"))
        {
            Kamikaze2 enemyHealth = collision.GetComponent<Kamikaze2>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(100f);
            }

            DeactivateBullet();
        }

        if (collision.gameObject.CompareTag("EnemyShip"))
        {
            EnemyFlyThrough enemyHealth = collision.GetComponent<EnemyFlyThrough>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(100f);
            }
            DeactivateBullet();
        }

        if (collision.gameObject.CompareTag("1stBoss"))
        {
            BossMovement bossHealth = collision.GetComponent<BossMovement>();
            if (bossHealth != null)
            {
                float damageAmount = bossHealth.maxHealth * 0.04f;
                bossHealth.TakeDamage(damageAmount);
            }

            DeactivateBullet();
        }

        if (collision.gameObject.CompareTag("2ndBoss"))
        {
            Boss_2_movement bossHealth = collision.GetComponent<Boss_2_movement>();
            if (bossHealth != null)
            {
                float damageAmount = bossHealth.maxHealth * 0.017f;
                bossHealth.TakeDamage(damageAmount);
            }

            DeactivateBullet();
        }

        if (collision.gameObject.CompareTag("3rdBoss"))
        {
            Boss3 bossHealth = collision.GetComponent<Boss3>();
            if (bossHealth != null)
            {
                float damageAmount = bossHealth.maxHealth * 0.008f;
                bossHealth.TakeDamage(damageAmount);
            }

            DeactivateBullet();
        }

        if (collision.gameObject.tag == "Boundary")
        {
            DeactivateBullet();
        }

        if (collision.gameObject.tag == "Meteorito")
        {
            Meteorito meteoritoHealth = collision.GetComponent<Meteorito>();
            if (meteoritoHealth != null)
            {
                float damageAmount = meteoritoHealth.life * 0.34f;
                meteoritoHealth.TakeDamage(damageAmount);
            }
        }

        if (collision.gameObject.CompareTag("ProyectileBoss2"))
        {
            Misil_boss_2 health = collision.GetComponent<Misil_boss_2>();
            if (health != null)
            {

                health.TakeDamage(1f);
            }

            DeactivateBullet();
        }

        {
            if (collision.gameObject.CompareTag("EnemyPhasatron") ||
                collision.gameObject.CompareTag("EnemyWall") ||
                collision.gameObject.CompareTag("CrazyKamikaze") ||
                collision.gameObject.CompareTag("EnemyKamikaze") ||
                collision.gameObject.CompareTag("EnemyShip") ||
                collision.gameObject.CompareTag("1stBoss") ||
                collision.gameObject.CompareTag("2ndBoss") ||
                collision.gameObject.CompareTag("3rdBoss"))
            {
                // Aumentar la barra de carga al impactar con un enemigo
                if (projectileShoot != null)
                {
                    projectileShoot.IncreaseCharge();
                }

                DeactivateBullet();
            }
        }
    }



    private IEnumerator DeactivateAfterTime(float lifespan)
    {
        yield return new WaitForSeconds(lifespan);
        DeactivateBullet();
    }


    void DeactivateBullet()
    {
        if (bulletPool != null)
        {
            bulletPool.ReturnBullet(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
  

    void OnDisable()
    {
        StopAllCoroutines();
    }
}
