using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile2 : MonoBehaviour
{
    public GameManager gameManager;
    public float moveSpeed;
    public float rangeShoot;
    public float damage;
    public float lifespan = 1f;


    private PoolBulletPlayer bulletPool;

    public AudioClip shootSound;          // Sonido al disparar
    public AudioClip impactSound;         // Sonido al impactar
    private AudioSource audioSource;      // Componente para reproducir sonido

    public Vector2 moveDirection;

    void OnEnable()
    {
        // Encontrar el Pool solo si no lo has asignado previamente
        if (bulletPool == null)
        {
            bulletPool = FindObjectOfType<PoolBulletPlayer>();
        }


        // Obtener el componente AudioSource
        audioSource = GetComponent<AudioSource>();

        // Reproducir el sonido de disparo
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // Iniciar corrutina que desactiva la bala después de su tiempo de vida
        StartCoroutine(DeactivateAfterTime(lifespan));
    }

    void Update()
    {
        transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyPhasatron"))
        {
            Phasatron enemy = collision.gameObject.GetComponent<Phasatron>();
            Phasatron2 enemy2 = collision.gameObject.GetComponent<Phasatron2>();
            Phasatron3 enemy3 = collision.gameObject.GetComponent<Phasatron3>();


            if (enemy != null)
            {
                float damage = enemy.maxHealth * 0.13f;
                enemy.TakeDamage(damage); // Aplicar daño
            }
            if (enemy2 != null)
            {
                float damage = enemy2.maxHealth * 0.10f;
                enemy2.TakeDamage(damage); // Aplicar daño
            }
            if (enemy3 != null)
            {
                float damageAmount = enemy3.maxHealth * 0.07f;
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
                float damageAmount = healthPared.maxHealth * 0.30f;
                healthPared.TakeDamage(damageAmount); // Aplicar daño
            }
            if (healthPared2 != null)
            {
                float damageAmount = healthPared2.maxHealth * 0.27f;
                healthPared2.TakeDamage(damageAmount); // Aplicar daño
            }
            DeactivateBullet();
        }

        if (collision.gameObject.CompareTag("EnemyKamikaze"))
        {
            // Acceder al componente de salud del enemigo
            Kamikaze enemyHealth = collision.GetComponent<Kamikaze>();
            Kamikaze2 enemyHealth2 = collision.GetComponent<Kamikaze2>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(40f);
            }
            if (enemyHealth2 != null)
            {
                enemyHealth2.TakeDamage(37f);
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
                float damage = bossHealth.maxHealth * 0.01f;
                bossHealth.TakeDamage(damage);
            }

            DeactivateBullet();
        }

        if (collision.gameObject.CompareTag("2ndBoss"))
        {
            Boss_2_movement bossHealth = collision.GetComponent<Boss_2_movement>();
            if (bossHealth != null)
            {
                float damageAmount = bossHealth.maxHealth * 0.009f;
                bossHealth.TakeDamage(damageAmount);
            }

            DeactivateBullet();
        }

        if (collision.gameObject.CompareTag("3rdBoss"))
        {
            Boss3 bossHealth = collision.GetComponent<Boss3>();
            if (bossHealth != null)
            {
                float damageAmount = bossHealth.maxHealth * 0.005f;
                bossHealth.TakeDamage(damageAmount);
            }

            DeactivateBullet();
        }

        if (collision.gameObject.CompareTag("ProyectileBoss2"))
        {
           Misil_boss_2 health = collision.GetComponent<Misil_boss_2>();
            if (health != null)
            {
               
                health.TakeDamage(0.5f);
            }

            DeactivateBullet();
        }

        if (collision.gameObject.tag == "Boundary")
        {
            DeactivateBullet();
        }
    }
    private IEnumerator DeactivateAfterTime(float lifespan)
    {
        yield return new WaitForSeconds(lifespan); // Esperar el tiempo de vida
        DeactivateBullet(); // Desactivar la bala
    }

    void DeactivateBullet()
    {
        if (bulletPool != null)
        {
            bulletPool.ReturnBullet(gameObject);  // Recicla la bala
        }
        else
        {
            gameObject.SetActive(false);  // Desactiva si no hay pool
        }
    }

    void OnDisable()
    {
        // Asegúrate de que no se invoca más la función de desactivación
        StopAllCoroutines(); // Detener cualquier corrutina activa al desactivarse
    }
}
