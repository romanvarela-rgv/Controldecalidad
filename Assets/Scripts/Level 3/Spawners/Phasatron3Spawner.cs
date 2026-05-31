using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phasatron3Spawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab del enemigo
    [SerializeField] public float spawnInterval = 1f;

    public Transform[] moveSpots1; // Puntos de movimiento para el enemigo 1

    public int maxEnemies = 2; // Se permiten dos enemigos simultáneamente
    private int currentEnemies = 0;

    public int minScoreThreshold; // Puntaje mínimo para empezar a spawnear
    public int maxScoreThreshold; // Puntaje máximo para detener el spawn

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemies), spawnInterval, spawnInterval);
    }

    void SpawnEnemies()
    {
        // Solo spawnea si el puntaje está dentro del umbral y no se ha alcanzado el límite de enemigos
        if (GameManager.Instance.TotalScore >= minScoreThreshold && GameManager.Instance.TotalScore < maxScoreThreshold && currentEnemies < maxEnemies)
        {
            if (currentEnemies < 2)
            {
                SpawnEnemy(1); // Spawnea el enemigo 1 con sus puntos de movimiento
            }
        }
    }

    void SpawnEnemy(int enemyNumber)
    {
        float spawnPosX = Random.Range(-6, 3); // Variación en X
        float spawnPosY = 3.88f; // Posición fija en Y
        Vector2 spawnPosition = new Vector2(spawnPosX, spawnPosY);

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        Phasatron3 enemyScript = newEnemy.GetComponent<Phasatron3>();
        if (enemyNumber == 1)
        {
            enemyScript.moveSpots = moveSpots1; // Asignar puntos de movimiento del enemigo 1
        }

        currentEnemies++;
    }

    public void EnemyDestroyed()
    {
        currentEnemies--;
    }
}