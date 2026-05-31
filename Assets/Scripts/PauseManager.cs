using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject PauseBox;
    public bool onPause;
    private GlobalMusicManager musicManager;

    private void Start()
    {
        musicManager = FindObjectOfType<GlobalMusicManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (onPause)
            {
                Continue();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        onPause = true;
        PauseBox.SetActive(true);
        Time.timeScale = 0f;
        musicManager.PauseMusic();
    }

    public void Continue()
    {
        onPause = false;
        PauseBox.SetActive(false);
        Time.timeScale = 1f;
        musicManager.ResumeMusic();
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }
}
