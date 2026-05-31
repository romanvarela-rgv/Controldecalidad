using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.Collections;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] public HUD hud; 
    public int TotalScore { get { return totalScore; } }

    private float lives = 3;
    private bool playerInvulnerable = false;
    private float lastDamageTime = -2f;

    public int meteorScoreThreshold;
    public int bossScoreThreshold;
    public int stormScoreThreshold; 
    private int totalScore = 0;

    private bool bossInvoked = false;
    private bool stormActivated = false; 
    private bool healthBarBoss = false;

    public GameObject boss;
    public GameObject lifeBarBoss;

    private bool isImmortal = false;

    public MeteoritoSpawner[] meteoritoSpawners; 
    public MeteoritoSpawnerL[] meteoritoSpawnersL;
    public StormManager stormManager; 

    public float meteoritoRandomizeInterval = 5f; 

    public GameObject player; 

    [SerializeField] private VideoPlayer eventVideoPlayer; // VideoPlayer que reproducirá el video del evento

    public TMPro.TextMeshPro ScoreText;
    public TMPro.TextMeshPro HighScore;
    public TMPro.TextMeshPro FinalScoreText;

    public int highScoreLevel1 = 0; // High score para el nivel 1
    public int highScoreLevel2 = 0; // High score para el nivel 2

    public int scoreLevel1;
    public int scoreLevel2;

    public AudioClip deathSound;
    private AudioSource audioSource;

    private bool bossMusicPlayed = false; // Controla si la música ya cambió

    public int LastLevelIndex { get; set; }
    public static bool CanSpawnEnemies { get; private set; }


    [Header("Sistemas de juego")]
    [SerializeField] private GameObject[] enemySpawners; 
    [SerializeField] private GameObject alliesSystem;
    [SerializeField] private bool tutorialCompleted;
    [SerializeField] private Slider alliesBarSlider; 
    [SerializeField] private ProjectileShoot projectileShoot;


    public bool TutorialCompleted { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);            
            return;
        }
        Instance = this;
        LoadHighScores();

        CanSpawnEnemies = false;

        SetSpawnersActive(false);

        if (alliesBarSlider != null)
            alliesBarSlider.value = alliesBarSlider.maxValue;

        if (projectileShoot != null)
        {
            projectileShoot.ForceFullCharge();
        }
    }

    public void Start()
    {
        boss.SetActive(false);
        InvokeRepeating(nameof(RandomizeMeteoritoSpawners), meteoritoRandomizeInterval, meteoritoRandomizeInterval);

        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 1)
        {
            TutorialCompleted = true;
            OnTutorialCompleted(); // activa spawners, aliados, etc.
        }

            audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleImmortality();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActivateCheat();
        }
    }

    public void OnTutorialCompleted()
    {
        tutorialCompleted = true;

        CanSpawnEnemies = true;

        SetSpawnersActive(true);
        if (alliesSystem != null)
            alliesSystem.SetActive(true);
    }

    private void SetSpawnersActive(bool state)
    {
        foreach (GameObject spawner in enemySpawners)
        {
            if (spawner != null)
                spawner.SetActive(state);
        }
    }


    public void SetPlayerInvulnerable(bool state)
    {
        playerInvulnerable = state;
    }

    public void AddScore(int ScoreToAdd)
    {
        totalScore += ScoreToAdd;
        hud.UpdateScore(totalScore);


        PlayerPrefs.SetInt("TotalScore", totalScore);
        PlayerPrefs.Save();

        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (totalScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", totalScore);
            PlayerPrefs.Save();
        }

        if (!stormActivated && totalScore >= stormScoreThreshold)
        {
            ActivateStorm();
        }

        if (!bossInvoked && totalScore >= bossScoreThreshold)
        {
            InvokeBoss();
        }
    }

    private void ActivateStorm()
    {
        if (stormManager != null)
        {
            stormActivated = true; 
            stormManager.StartStorm(); 
        }
    }

    public void LoseLive(float damage = 1)
    {
        if (isImmortal)
        {
            return;
        }

        if (Time.time - lastDamageTime < 2f)
        {
            return;
        }

        lastDamageTime = Time.time;

        if (playerInvulnerable)
        {
            return;
        }

        lives -= damage;
        hud.UpdateLivesSlider(lives);

        if (player != null)
        {
            StartCoroutine(FlashRedAndInvulnerability());
        }

        if (lives <= 0)
        {
            PlayDeathSound();

            int currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

            LastLevelStore.Set(currentScene);

            CheckHighScore();
            StartCoroutine(WaitAndLoadScene(5));
        }
    }

    public IEnumerator FlashRedAndInvulnerability()
    {
        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.55f);

            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.1f);
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.6f);
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.1f);
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.6f);
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.1f);


            yield return new WaitForSeconds(0.8f);

            // Restaurar el color y la opacidad original
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        }

    }


    private IEnumerator WaitAndLoadScene(int sceneIndex)
    {
        yield return new WaitForSeconds(2f); 
        SceneManager.LoadScene(sceneIndex); 
    }


    public bool RecoverLive()
    {
        if (lives < hud.GetMaxLives())
        {
            lives = Mathf.Min(lives + 1, hud.GetMaxLives());
            hud.UpdateLivesSlider(lives);
            return true;
        }

        return false;
    }

    private void ToggleImmortality()
    {
        isImmortal = !isImmortal;
        hud.SetLivesSliderVisibility(!isImmortal);
    }

    private void ActivateCheat()
    {
        totalScore += 900;
        hud.UpdateScore(totalScore);

        if (!bossInvoked)
        {
            InvokeBoss();
        }
    }

    public bool IsPlayerImmortal()
    {
        return isImmortal;
    }

    public void SaveScore(int level, int score)
    {
        if (level == 1)
        {
            scoreLevel1 = score;
            PlayerPrefs.SetInt("ScoreLevel1", scoreLevel1);
        }
        else if (level == 2)
        {
            scoreLevel2 = score;
            PlayerPrefs.SetInt("ScoreLevel2", scoreLevel2);
        }

        PlayerPrefs.Save();
    }
    public void CheckHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0); // Obtiene el high score guardado
        if (totalScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", totalScore); // Guarda el nuevo high score
            PlayerPrefs.Save();
        }
    }

    public void SaveHighScore(int level, int score)
    {
        if (level == 1 && score > highScoreLevel1)
        {
            highScoreLevel1 = score;
            PlayerPrefs.SetInt("HighScoreLevel1", highScoreLevel1);
        }
        else if (level == 2 && score > highScoreLevel2)
        {
            highScoreLevel2 = score;
            PlayerPrefs.SetInt("HighScoreLevel2", highScoreLevel2);
        }
    }

    private void LoadHighScores()
    {
        highScoreLevel1 = PlayerPrefs.GetInt("HighScoreLevel1", 0);
        highScoreLevel2 = PlayerPrefs.GetInt("HighScoreLevel2", 0);
    }

    public void EndLevel(int level)
    {
        int score = totalScore; // Usa el totalScore acumulado en la partida

        SaveScore(level, score);       // Guarda el score del nivel
        SaveHighScore(level, score);   // Guarda el highscore del nivel
    }

    private void RandomizeMeteoritoSpawners()
    {
        if (bossInvoked) return; // Si el jefe está invocado, no se randomizan los spawners

        foreach (var spawner in meteoritoSpawners)
        {
            if (spawner != null)
            {
                spawner.gameObject.SetActive(Random.value > 0.5f); // Activa/desactiva aleatoriamente
            }
        }

        foreach (var spawnerL in meteoritoSpawnersL)
        {
            if (spawnerL != null)
            {
                spawnerL.gameObject.SetActive(Random.value > 0.5f); 
            }
        }
    }

    public void InvokeBoss()
    {
        bossInvoked = true;
        healthBarBoss = true;

        StartCoroutine(InvokeBossWithDelay());
    }

    private IEnumerator InvokeBossWithDelay()
    {

        yield return new WaitForSeconds(1f);
        if (eventVideoPlayer != null)
        {
            eventVideoPlayer.gameObject.SetActive(true); // Activar el VideoPlayer
            eventVideoPlayer.Play(); // Reproducir el video

            // Esperar 5 segundos para que termine el video
            yield return new WaitForSeconds(5f);

            eventVideoPlayer.Stop(); // Detener el video
            eventVideoPlayer.gameObject.SetActive(false); // Desactivar el VideoPlayer
        }

        yield return new WaitForSeconds(0.3f);

        // Spawn del jefe
        if (boss != null)
        {
            boss.transform.position = new Vector3(0, 2.3f, 0);
            boss.SetActive(true);
        }

        if (lifeBarBoss != null)
        {
            lifeBarBoss.SetActive(true);
        }

        CancelInvoke(nameof(RandomizeMeteoritoSpawners));

        foreach (var spawner in meteoritoSpawners)
        {
            if (spawner != null)
            {
                spawner.gameObject.SetActive(false);
            }
        }

        foreach (var spawnerL in meteoritoSpawnersL)
        {
            if (spawnerL != null)
            {
                spawnerL.gameObject.SetActive(false);
            }
        }
    }

    public void ResetTotalScore()
    {
        totalScore = 0;
        hud.UpdateScore(totalScore); 
    }

    private void PlayDeathSound()
    {
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
    }

    public bool IsPlayerInvulnerable()
    {
        return playerInvulnerable;
    }
}
