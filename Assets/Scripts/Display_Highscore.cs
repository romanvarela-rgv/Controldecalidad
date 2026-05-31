using UnityEngine;
using TMPro; // Importar TextMeshPro

public class HighScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI highScoreText; // Asigna en el Inspector

    void Start()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "" + highScore;
    }
}