using System.Collections;
using System.Threading;
using UnityEngine;

public class EnemyFlyThroughSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject enemyFlyThroughPrefab;

    [Header("Timing")]
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float firstSpawnDelay = 3f; 

    [Header("Condición por Score")]
    [SerializeField] private int minScoreThreshold = 0;
    [SerializeField] private int maxScoreThreshold = int.MaxValue;

    [SerializeField] private int maxActiveEnemies = 6;

    [Header("Posiciones")]
    [SerializeField] private float topY = 6f;
    [SerializeField] private Vector2 topXRange = new(-7f, 7f);

    [SerializeField] private float sideXLeft = -9f;
    [SerializeField] private float sideXRight = 8f;
    [SerializeField] private Vector2 sideYRange = new(-2f, 2f);

    private int activeEnemies = 0;
    private int lastSpawnSide = -1; 

    private Coroutine loop;
    private WaitForSeconds wait;

    private float timer;
    private bool firstSpawn = true;

    private void OnValidate()
    {
        spawnInterval = Mathf.Max(0.01f, spawnInterval);

        if (topXRange.x > topXRange.y)
            (topXRange.x, topXRange.y) = (topXRange.y, topXRange.x);

        if (sideYRange.x > sideYRange.y)
            (sideYRange.x, sideYRange.y) = (sideYRange.y, sideYRange.x);

        if (maxScoreThreshold < minScoreThreshold)
            maxScoreThreshold = minScoreThreshold;
    }

    private void OnEnable()
    {

        timer = firstSpawn ? firstSpawnDelay : spawnInterval;

        wait = new WaitForSeconds(spawnInterval);
        loop = StartCoroutine(SpawnLoop());
    }

    private void OnDisable()
    {
        if (loop != null) StopCoroutine(loop);
        loop = null;
    }

    private void Update()
    {
        // BLOQUEO ABSOLUTO
        if (!GameManager.CanSpawnEnemies)
            return;

        if (enemyFlyThroughPrefab == null)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            TrySpawn();
            firstSpawn = false;
            timer = spawnInterval;
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (enabled)
        {
            TrySpawn();
            yield return wait;
        }
    }

    private void TrySpawn()
    {
        if (enemyFlyThroughPrefab == null) return;

        int score = GameManager.Instance != null ? GameManager.Instance.TotalScore : 0;
        if (score < minScoreThreshold || score >= maxScoreThreshold) return;

        // Mantengo tu condición original (puede ser bug):
        if (activeEnemies >= maxScoreThreshold) return;

        // Si querés la corrección, usá esto:
        // if (activeEnemies >= maxActiveEnemies) return;

        Vector2 spawnPos;
        Vector2 moveDir;
        GetBalancedSpawnPositionAndDirection(out spawnPos, out moveDir);

        GameObject enemy = Instantiate(enemyFlyThroughPrefab, spawnPos, Quaternion.identity);

        if (enemy.TryGetComponent<EnemyFlyThrough>(out var fly))
        {
            fly.SetMoveDirection(moveDir);
        }

        activeEnemies++;
    }

    private void GetBalancedSpawnPositionAndDirection(out Vector2 spawnPosition, out Vector2 moveDirection)
    {
        int spawnSide;
        do
        {
            spawnSide = Random.Range(0, 3); // 0 = top, 1 = left, 2 = right
        } while (spawnSide == lastSpawnSide);

        lastSpawnSide = spawnSide;

        switch (spawnSide)
        {
            case 0: // Top
                spawnPosition = new Vector2(Random.Range(topXRange.x, topXRange.y), topY);
                moveDirection = Vector2.down;
                break;
            case 1: // Left
                spawnPosition = new Vector2(sideXLeft, Random.Range(sideYRange.x, sideYRange.y));
                moveDirection = Vector2.right;
                break;
            case 2: // Right
                spawnPosition = new Vector2(sideXRight, Random.Range(sideYRange.x, sideYRange.y));
                moveDirection = Vector2.left;
                break;
            default:
                spawnPosition = new Vector2(0f, topY);
                moveDirection = Vector2.down;
                break;
        }
    }

    public void EnemyDestroyed()
    {
        activeEnemies = Mathf.Max(0, activeEnemies - 1);
    }
}
