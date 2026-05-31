using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text totalScoreText;

    void Awake()
    {
        // 1) Si no está seteado en el Inspector, intento encontrarlo en este GO
        if (!totalScoreText && TryGetComponent(out TMP_Text selfText))
            totalScoreText = selfText;

        // 2) Si tampoco, lo busco en hijos (incluye inactivos)
        if (!totalScoreText)
            totalScoreText = GetComponentInChildren<TMP_Text>(true);

        // 3) Si sigue sin aparecer, log claro y desactivo este script para evitar NRE
        if (!totalScoreText)
        {
            Debug.LogError($"[ScoreDisplay] Falta referencia a TMP_Text en '{name}'. " +
                           "Asigná el Text en el Inspector o poné este script en el mismo objeto del texto.", this);
            enabled = false;
        }
    }

    void Start()
    {
        if (!enabled) return; // si falló Awake

        int totalScore = PlayerPrefs.GetInt("TotalScore", 0);
        totalScoreText.text = totalScore.ToString();
    }
}
