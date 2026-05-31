using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Levels : MonoBehaviour
{
    private float delayTime = 1f; // Tiempo de espera

    public void Level1()
    {
        Invoke("LoadLevel1", delayTime); // Llama a la escena Survival
    }

    public void Level2()
    {
        Invoke("LoadLevel2", delayTime); // Llama a la escena Level 1
    }

    public void Level3()
    {
        Invoke("LoadLevel3", delayTime); // Llama a la escena Level 1
    }

    public void Back()
    {
        Invoke("LoadBack", delayTime); // Vuelve al menú principal
    }

    public void Exit()
    {
        Invoke("QuitGame", delayTime); // Cierra el juego
    }

    // Métodos privados para cargar las escenas
    private void LoadLevel1()
    {
        SceneManager.LoadScene(2);
    }

    private void LoadLevel2()
    {
        SceneManager.LoadScene(3);
    }

    private void LoadLevel3()
    {
        SceneManager.LoadScene(6);
    }


    private void LoadBack()
    {
        SceneManager.LoadScene(0);
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
