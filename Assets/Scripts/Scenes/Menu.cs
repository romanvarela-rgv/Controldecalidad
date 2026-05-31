using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private float delayTime = 1f;

    [SerializeField] private string mainMenuScene = "Menu";
    [SerializeField] private string levelSelectScene = "Select Levels";
    [SerializeField] private string level1Scene = "Level1";
    [SerializeField] private string level2Scene = "Level2";
    [SerializeField] private string level3Scene = "Level3";
    [SerializeField] private string defeatScene = "GameOverScreen";

    public void StartGame() => LoadSceneAfterDelay(level1Scene);
    public void LevelSelection() => LoadSceneAfterDelay(levelSelectScene);
    public void Level1() => LoadSceneAfterDelay(level1Scene);
    public void Level2() => LoadSceneAfterDelay(level2Scene);
    public void Level3() => LoadSceneAfterDelay(level3Scene);

    public void BackMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CheckHighScore();
        }
        LoadSceneAfterDelay(mainMenuScene);
    }

    public void Exit()
    {
        StartCoroutine(QuitAfterDelay());
    }

    public void RetryLastLevel()
    {
        int idx = LastLevelStore.Get();
        Debug.Log($"[Menu] RetryLastLevel -> idx={idx}");

        if (idx >= 0)
        {
            StartCoroutine(LoadAfterDelay(idx)); 
        }
        else
        {
            LoadSceneAfterDelay(levelSelectScene);
        }
    }

    public void NextLevel()
    {
        string lastCompleted = LevelProgressStore.GetLastCompletedName();
        Debug.Log($"[Menu] NextLevel desde: {lastCompleted}");

        if (lastCompleted == level1Scene) LoadSceneAfterDelay(level2Scene);
        else if (lastCompleted == level2Scene) LoadSceneAfterDelay(level3Scene);
        else
            LoadSceneAfterDelay(levelSelectScene); 
    }

    private void LoadSceneAfterDelay(string sceneName)
    {
        StartCoroutine(LoadAfterDelay(sceneName));
    }

    private IEnumerator LoadAfterDelay(string sceneName)
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator LoadAfterDelay(int buildIndex)
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(buildIndex);
    }

    private IEnumerator QuitAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);
        Application.Quit();
    }
}
