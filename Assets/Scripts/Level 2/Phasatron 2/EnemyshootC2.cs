using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class EnemyshootC2 : MonoBehaviour
{
    public GameObject EnemyBulletGo2;
    public float fireRate = 3f;
    [SerializeField]private float fireCooldown;  // Tiempo restante hasta el próximo disparo

    public AudioSource ClipAttack;

    void Start()
    {

    }

    void Update()
    {
        // Actualizar el temporizador en cada frame
        fireCooldown -= Time.deltaTime;

        // Dispara solo cuando el cooldown llega a 0
        if (fireCooldown <= 0f)
        {
            FireEnemyBullet();
            fireCooldown = fireRate; // Reiniciar el temporizador
        }
    }

    //Function to fire an enemy bullet
    void FireEnemyBullet()
    {
        
        //get a reference to the player's ship
        GameObject playerShip = GameObject.Find("Player");
        GameObject playerShip2 = GameObject.Find("Player Fast");

        ClipAttack.Play();
        if (playerShip != null)
        {
            GameObject bullet = (GameObject)Instantiate(EnemyBulletGo2);

            bullet.transform.position = transform.position;

            Vector2 direction = playerShip.transform.position - bullet.transform.position;

            bullet.GetComponent<EnemyBullet>().SetDirection(direction);
        }

        if (playerShip2 != null)
        {
            GameObject bullet = (GameObject)Instantiate(EnemyBulletGo2);

            bullet.transform.position = transform.position;

            Vector2 direction = playerShip2.transform.position - bullet.transform.position;

            bullet.GetComponent<EnemyBullet>().SetDirection(direction);
        }
    }

}
