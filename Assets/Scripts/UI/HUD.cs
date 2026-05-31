using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI Score;
    [SerializeField] public Slider livesSlider; // Referencia al slider de vidas

    // flags para no spamear warnings
    private bool _warnedNoScore, _warnedNoGM, _warnedNoSlider;

    void Awake()
    {
        // Autowire defensivo (encuentra en este GO o hijos, incluso inactivos)
        if (!Score)
        {
            if (!TryGetComponent(out TextMeshProUGUI selfTMP))
                selfTMP = GetComponentInChildren<TextMeshProUGUI>(true);
            if (selfTMP) Score = selfTMP;
        }

        if (!livesSlider)
            livesSlider = GetComponentInChildren<Slider>(true);

        if (!Score && !_warnedNoScore)
        {
            _warnedNoScore = true;
            Debug.LogWarning("[HUD] Falta referencia a TextMeshProUGUI 'Score'. Asigná el texto en el Inspector o poné este script en el mismo objeto del texto.", this);
        }

        if (!livesSlider && !_warnedNoSlider)
        {
            _warnedNoSlider = true;
            Debug.LogWarning("[HUD] Falta referencia a Slider de vidas. No se actualizará la UI de vidas.", this);
        }
    }

    void Start()
    {
        // Configuración inicial del slider si existe
        if (livesSlider)
        {
            livesSlider.maxValue = 3; // valor máximo del slider
            livesSlider.value = 3;    // valor inicial
        }
    }

    void Update()
    {
        var gm = GameManager.Instance; // puede ser null si aún no existe/persistió

        if (Score != null && gm != null)
        {
            Score.text = gm.TotalScore.ToString();
        }
        else
        {
            if (Score == null && !_warnedNoScore)
            {
                _warnedNoScore = true;
                Debug.LogWarning("[HUD] 'Score' es null en Update().", this);
            }
            if (gm == null && !_warnedNoGM)
            {
                _warnedNoGM = true;
                Debug.LogWarning("[HUD] GameManager.Instance es null en Update(). Asegurá su creación y/o DontDestroyOnLoad antes de usarlo.", this);
            }
        }
    }

    public void UpdateScore(int totalScore)
    {
        if (Score) Score.text = totalScore.ToString();
    }

    public void UpdateLivesSlider(float lives)
    {
        if (livesSlider) livesSlider.value = lives;
    }

    public void SetLivesSliderVisibility(bool isVisible)
    {
        if (livesSlider) livesSlider.gameObject.SetActive(isVisible);
    }

    public float GetMaxLives()
    {
        return livesSlider ? livesSlider.maxValue : 0f;
    }

#if UNITY_EDITOR
    // Calidad de vida en editor: auto-asigna al modificar el prefab/inspector
    void OnValidate()
    {
        if (!Score)
        {
            var tmp = GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp) Score = tmp;
        }
        if (!livesSlider)
            livesSlider = GetComponentInChildren<Slider>(true);
    }
#endif
}
