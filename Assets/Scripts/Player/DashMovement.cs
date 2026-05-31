using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DashMovement : MonoBehaviour
{
    public float moveSpeed = 4f; // Velocidad máxima
    public float acceleration = 15f; // Aceleración del movimiento
    public float deceleration = 10f; // Desaceleración al frenar
    public float hInput;
    public float vInput;

    private bool isDashing = false;
    public float dashingPower = 7f;
    public float dashingTime = 0.4f;
    private float dashingCooldown = 1f;
    public float chargeSpeed = 0.5f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Slider dashChargeSlider;

    private Vector2 movementInput;
    private Vector2 currentVelocity;

    private float screenLeft = -8.2f;
    private float screenRight = 8.2f;
    private float screenTop = 4.2f;
    private float screenBottom = -3.7f;

    private float slowdownPlayer = 0.3f;

    private float originalMoveSpeed;

    // Sonido de muerte
    //public AudioClip deathSound;
    //private AudioSource audioSource;

    //public PauseManager pauseM;

    private void Start()
    {
        originalMoveSpeed = moveSpeed;
    }

    private void Update()
    {
        /*
        if(pauseM.onPause == true)
        {
            return;
        }
        */

        if (isDashing)
        {
            return;
        }
        
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        movementInput = new Vector2(hInput, vInput).normalized;

        if (dashChargeSlider.value < dashChargeSlider.maxValue)
        {
            dashChargeSlider.value += Time.deltaTime * chargeSpeed;
        }

        if (Input.GetKeyDown(KeyCode.Space) && dashChargeSlider.value >= dashChargeSlider.maxValue)
        {
            StartCoroutine(Dash(movementInput));
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        if (movementInput.magnitude > 0)
        {
            Vector2 targetVelocity = movementInput * moveSpeed;
            currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }

        rb.velocity = currentVelocity;

        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, screenLeft, screenRight);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, screenBottom, screenTop);
        transform.position = clampedPosition;
    }

    private IEnumerator Dash(Vector2 direction)
    {
        isDashing = true;
        GameManager.Instance.SetPlayerInvulnerable(true);

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        Vector2 dashDirection = direction.normalized * dashingPower;
        rb.velocity = dashDirection;

        yield return new WaitForSeconds(dashingTime);

        rb.gravityScale = originalGravity;
        isDashing = false;

        GameManager.Instance.SetPlayerInvulnerable(false);
        dashChargeSlider.value = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing && collision.gameObject.CompareTag("EnemyKamikaze") || collision.gameObject.CompareTag("CrazyKamikaze"))
        {
            GameManager.Instance.AddScore(20);
            Destroy(collision.gameObject);
        }

        if (isDashing && collision.gameObject.CompareTag("EnemyShip"))
        {
            GameManager.Instance.AddScore(5);
            Destroy(collision.gameObject);
        }

        if (isDashing && collision.gameObject.CompareTag("EnemyWall"))
        {
            EnemyWall healthPared = collision.gameObject.GetComponent<EnemyWall>();
            EnemyWall2 healthPared2 = collision.gameObject.GetComponent<EnemyWall2>();

            float damageAmount = healthPared.maxHealth * 0.50f;
            healthPared.TakeDamage(damageAmount);
            if (healthPared == null)
            {
                Destroy(collision.gameObject);
                GameManager.Instance.AddScore(10);
            }
            float damageAmount2 = healthPared2.maxHealth * 0.45f;
            healthPared2.TakeDamage(damageAmount2);
            if (healthPared2 == null)
            {
                Destroy(collision.gameObject);
                GameManager.Instance.AddScore(17);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (!GameManager.Instance.IsPlayerInvulnerable())
        {
            StartCoroutine(SlowDownPlayer());
        }

    }

    private IEnumerator SlowDownPlayer()
    {
        moveSpeed *= slowdownPlayer; // Reduce la velocidad a la mitad
        yield return new WaitForSeconds(2f); // Duración de la ralentización
        moveSpeed = originalMoveSpeed; // Restaura la velocidad original
    }
}
