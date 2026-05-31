using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeSpawner : MonoBehaviour
{
    [SerializeField] public GameObject enemyPrefab;

    [SerializeField] private float _minimumSpawnTime = 0.5f;
    [SerializeField] private float _maximumSpawnTime = 1.5f;

    private float _timeUntilSpawn;
    private bool stopSpawning = false;

    public int minScoreThreshold;  // Puntaje mínimo para empezar a spawnear
    public int maxScoreThreshold;  // Puntaje máximo para detener el spawn

    // flags para no spamear logs
    private bool _warnedNoGM, _warnedNoPrefab;

    void Awake()
    {
        SetTimeUntilSpawn();
    }

    void Update()
    {
        // Cachear referencia y validar que exista (puede ser null al inicio o en cambio de escena)
        var gm = GameManager.Instance;
        if (gm == null)
        {
            if (!_warnedNoGM)
            {
                _warnedNoGM = true;
                Debug.LogWarning("[KamikazeSpawner] GameManager.Instance es null en Update(). " +
                                 "Asegurá su creación/DontDestroyOnLoad antes de usarlo.", this);
            }
            return; // nada que hacer sin GM
        }

        // Habilitar spawn solo si el score está en rango
        stopSpawning = !(gm.TotalScore >= minScoreThreshold && gm.TotalScore < maxScoreThreshold);

        if (stopSpawning) return;

        _timeUntilSpawn -= Time.deltaTime; // cuenta regresiva por frame

        if (_timeUntilSpawn <= 0f)
        {
            SpawnKamikaze();
            SetTimeUntilSpawn();
        }
    }

    void SpawnKamikaze()
    {
        if (enemyPrefab == null)
        {
            if (!_warnedNoPrefab)
            {
                _warnedNoPrefab = true;
                Debug.LogError("[KamikazeSpawner] Falta 'enemyPrefab'. Asignalo en el Inspector.", this);
            }
            return;
        }

        // Forzamos Z=0 al instanciar
        Vector3 spawnPosition = transform.position;
        spawnPosition.z = 0f;

        GameObject newKamikaze = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        // Si necesitás el script, lo podés cachear:
        // var enemyScript = newKamikaze.GetComponent<Kamikaze>();
    }

    private void SetTimeUntilSpawn()
    {
        // Evitar valores no positivos y asegurar rango válido
        float min = Mathf.Max(0.01f, _minimumSpawnTime);
        float max = Mathf.Max(min, _maximumSpawnTime);
        _timeUntilSpawn = Random.Range(min, max); // float inclusivo [min..max]
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // Mantener datos coherentes al editar en Inspector
        if (_maximumSpawnTime < _minimumSpawnTime)
            _maximumSpawnTime = _minimumSpawnTime;
    }
#endif
}
