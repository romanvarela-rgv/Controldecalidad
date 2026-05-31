using System.Collections;
using UnityEngine;

public class LaserBoss : MonoBehaviour
{
    private Vector3 moveDirection;

    [Header("Movement & Damage")]
    [SerializeField] private float speed = 6f;     
    [SerializeField] private int damage = 1;      

    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 3f;  
    private Coroutine autoDisableRoutine;

    private void OnEnable()
    {
        if (lifeTime > 0f)
            autoDisableRoutine = StartCoroutine(AutoDisable(lifeTime));
    }

    private void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        var cam = Camera.main;
        if (cam != null)
        {
            Vector2 min = cam.ViewportToWorldPoint(new Vector2(0f, 0f));
            Vector2 max = cam.ViewportToWorldPoint(new Vector2(1f, 1f));

            Vector3 p = transform.position;
            if (p.x < min.x || p.x > max.x || p.y < min.y || p.y > max.y)
                gameObject.SetActive(false);
        }
    }

    public void SetMoveDirection(Vector3 dir)
    {
        moveDirection = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector3.zero;
    }

    private IEnumerator AutoDisable(float t)
    {
        yield return new WaitForSeconds(t);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (autoDisableRoutine != null)
        {
            StopCoroutine(autoDisableRoutine);
            autoDisableRoutine = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.LoseLive(damage);
            gameObject.SetActive(false);

        }
    }

    public void Initialize(Vector3 direction, float lifetimeOverride = -1f)
    {
        SetMoveDirection(direction);
        if (lifetimeOverride >= 0f) lifeTime = lifetimeOverride;
    }
}
