using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalMusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip level1Music;
    public AudioClip level2Music;
    public AudioClip level3Music;

    private static GlobalMusicManager instance;
    private bool isMusicPaused = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusicForCurrentScene();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForCurrentScene();
    }

    private void PlayMusicForCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        AudioClip newClip = null;

        if (currentSceneIndex == 2) newClip = level1Music;
        else if (currentSceneIndex == 3) newClip = level2Music;
        else if (currentSceneIndex == 4) newClip = level3Music;

        if (newClip != null && audioSource.clip != newClip)
        {
            audioSource.clip = newClip;
            audioSource.Play();
            isMusicPaused = false;
        }
        else if (newClip == null)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }

    public void PauseMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            isMusicPaused = true;
        }
    }

    public void ResumeMusic()
    {
        if (isMusicPaused)
        {
            audioSource.Play();
            isMusicPaused = false;
        }
    }
}
