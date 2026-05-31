using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class BossMovement : MonoBehaviour
{
    private float moveSpeed;
    private bool moveRight;
    private float waitTime;
    [SerializeField] private HealthbarBoss healthbar;
    [SerializeField] public float maxHealth;

    private SpriteRenderer spriteRenderer;

    private float health;
    private float currentHealth; 
    public int valueBoss = 200;

    public float speed;
    public float startWaitTime;

    public Transform[] MoveBoss;
    public int targetPoint;
    private int randomSpot;

    public AudioSource Clip;

    void Start()
    {
        health = maxHealth;
        healthbar.UpdateHealthbar(maxHealth, health);
        targetPoint = 0;
        waitTime = startWaitTime;
        randomSpot = Random.Range(0, MoveBoss.Length);

    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, MoveBoss[randomSpot].position, speed * Time.deltaTime);
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (Vector2.Distance(transform.position, MoveBoss[randomSpot].position) < 0.2f)
        {
            if (waitTime <= 0)
            {
                randomSpot = Random.Range(0, MoveBoss.Length);
                waitTime = startWaitTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        } 
    }

    public void TakeDamage(float damage)
    {
        StartCoroutine(DamageCoroutine(damage));
    }

    private IEnumerator DamageCoroutine(float damage)
    {
        health -= damage; 
        healthbar.UpdateHealthbar(maxHealth, health);

        if (health > 0)
        {
            Clip.Play();
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }

        if (health <= 0)
        {
            Destroy(gameObject); 
            GameManager.Instance.AddScore(valueBoss);

            var current = SceneManager.GetActiveScene();
            LevelProgressStore.SetLastCompletedName(current.name);

            SceneManager.LoadScene("WinScreen");
        }

        yield return null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.LoseLive(0.5f);
        }
    }
}