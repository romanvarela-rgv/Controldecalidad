using System.Collections;
using UnityEngine;

public class BulletBoss : MonoBehaviour
{
    private Vector2 moveDirection;
    [SerializeField] private float moveSpeed = 5f;

    private Coroutine autoDisableRoutine;

    private void OnEnable()
    {
        autoDisableRoutine = StartCoroutine(AutoDisable(3f));
    }

    private void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        if ((transform.position.x < min.x) || (transform.position.x > max.x) ||
            (transform.position.y < min.y) || (transform.position.y > max.y))
        {
            gameObject.SetActive(false);
        }
    }

    public void SetMoveDirection(Vector2 dir)
    {
        moveDirection = dir.normalized;
    }

    private IEnumerator AutoDisable(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (autoDisableRoutine != null)
            StopCoroutine(autoDisableRoutine);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.LoseLive(1f);
            gameObject.SetActive(false);

        }
    }
    public static void StopAndClearAll()
    {
        BulletBoss[] bullets = FindObjectsOfType<BulletBoss>(true);
        foreach (var b in bullets)
        {
            b.gameObject.SetActive(false);
        }
    }
}
