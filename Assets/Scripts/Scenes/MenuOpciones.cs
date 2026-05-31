using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MenuOpciones : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    private void Start()
    {
        // Recuperar el volumen guardado y aplicarlo al inicio
        if (PlayerPrefs.HasKey("Volumen"))
        {
            float volumenGuardado = PlayerPrefs.GetFloat("Volumen");
            audioMixer.SetFloat("Volumen", volumenGuardado);
        }
    }

    public void PantallaCompleta(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta;
    }

    public void CambiarVolumen(float volumen)
    {
        audioMixer.SetFloat("Volumen", volumen);

        // Guardar el volumen en PlayerPrefs
        PlayerPrefs.SetFloat("Volumen", volumen);
        PlayerPrefs.Save(); // Asegurarse de guardar inmediatamente
    }

    /*
    public void CambiarCalidad(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }
    */

    public void Volver()
    {
        SceneManager.LoadScene(0);
    }
}
