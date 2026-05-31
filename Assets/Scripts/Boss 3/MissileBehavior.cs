using UnityEngine;

public class MissileBehavior : MonoBehaviour
{
    private Transform target; // Objetivo del misil
    private float speed; // Velocidad del misil
    private float rotationSpeed; // Velocidad de rotación del misil

    public void Initialize(Transform targetTransform, float moveSpeed, float rotSpeed)
    {
        target = targetTransform;
        speed = moveSpeed;
        rotationSpeed = rotSpeed;
    }

    private void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("No hay objetivo. Destruyendo misil.");
            Destroy(gameObject);
            return;
        }

        Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
        Debug.Log($"Dirección hacia el jugador: {direction}");

        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        Debug.Log("Moviendo misil hacia adelante.");
        transform.Translate(Vector2.up * speed * Time.deltaTime, Space.Self);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Misil impactó al jugador.");
            GameManager.Instance.LoseLive(1); // Restar vida al jugador
            Destroy(gameObject);
        }

    }
}
