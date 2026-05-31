using System.Collections;
using UnityEngine;

public class PhasatronSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Timing")]
    [SerializeField] private float spawnInterval = 1f;

    [Header("Movimiento del enemigo")]
    [SerializeField] private Transform[] moveSpots;  

    [Header("Límites de spawn")]
    [SerializeField] private int maxEnemies = 1;
    private int currentEnemy = 0;

    [Header("Condición por Score")]
    [SerializeField] private int minScoreThreshold = 0;
    [SerializeField] private int maxScoreThreshold = int.MaxValue;

    [Header("Rango de aparición")]
    [SerializeField] private Vector2 spawnXRange = new(-6f, 3f);
    [SerializeField] private float spawnY = 3.88f;

    private Coroutine loop;
    private WaitForSeconds wait;

    private void OnValidate()
    {
        spawnInterval = Mathf.Max(0.01f, spawnInterval);
        maxEnemies = Mathf.Max(0, maxEnemies);

        if (spawnXRange.x > spawnXRange.y)
            (spawnXRange.x, spawnXRange.y) = (spawnXRange.y, spawnXRange.x);

        if (maxScoreThreshold < minScoreThreshold)
            maxScoreThreshold = minScoreThreshold;
    }

    private void OnEnable()
    {
        wait = new WaitForSeconds(spawnInterval);
        loop = StartCoroutine(SpawnLoop());
    }

    private void OnDisable()
    {
        if (loop != null) StopCoroutine(loop);
        loop = null;
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
        int score = GameManager.Instance != null ? GameManager.Instance.TotalScore : 0;
        if (score < minScoreThreshold || score >= maxScoreThreshold) return;
        if (currentEnemy >= maxEnemies) return;
        if (enemyPrefab == null) return;

        float spawnPosX = Random.Range(spawnXRange.x, spawnXRange.y);
        Vector2 spawnPosition = new(spawnPosX, spawnY);

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        if (newEnemy.TryGetComponent<Phasatron>(out var enemyScript))
        {
            enemyScript.moveSpots = moveSpots;
        }

        currentEnemy++;
    }

    public void EnemyDestroyed()
    {
        currentEnemy = Mathf.Max(0, currentEnemy - 1);
    }
}
