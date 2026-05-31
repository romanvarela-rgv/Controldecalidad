using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private float typeSpeed = 0.03f;

    [Header("Allies Bar")]
    [SerializeField] private Slider alliesBarSlider;
    [SerializeField] private float requiredAlliesFill = 1f;

    [Header("References")]
    [SerializeField] private ProjectileShoot projectileShoot;

    [Header("PowerUp Triple Shot")]
    [SerializeField] private GameObject tripleShotPowerUp;

    private int stepIndex = 0;

    // Paso 1
    private bool pressedW;
    private bool pressedA;
    private bool pressedS;
    private bool pressedD;

    // Paso 2
    private bool pressedShoot;

    // Paso 3
    private bool usedDash;

    // Paso 4
    private bool alliesActivated;

    // Paso 5
    private bool triplePicked;
    private bool tripleUsed;

    private bool isTyping;
    private bool isTransitioning; 
    private string currentFullText;
    private Coroutine typeRoutine;

    private void Start()
    {
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 1)
        {
            if (tutorialPanel != null)
                tutorialPanel.SetActive(false);

            enabled = false;
            return;
        }

        stepIndex = 0;

        if (tripleShotPowerUp != null)
            tripleShotPowerUp.SetActive(false);

        StartStep();
    }

    private void Update()
    {
        if (isTyping || isTransitioning)
            return;

        switch (stepIndex)
        {
            case 0:
                CheckMovement();
                break;
            case 1:
                CheckShoot();
                break;
            case 2:
                CheckDash();
                break;
            case 3:
                CheckAlliesAndF();
                break;
            case 4:
                CheckTripleShot();
                break;

        }
    }

    private void StartStep()
    {
        if (tutorialPanel != null)
            tutorialPanel.SetActive(true);

        switch (stepIndex)
        {
            case 0:
                SetTextProcedural("Mové la nave usando W, A, S y D en todas las direcciones.");
                break;

            case 1:
                SetTextProcedural("Dispará con la tecla P.");
                break;

            case 2:
                SetTextProcedural("Usá el dash con una tecla direccional (W/A/S/D) + ESPACIO.");
                break;

            case 3:
                SetTextProcedural("Llená la barra de aliados y presioná F para activarlos.");
                break;

            case 4:
                if (tripleShotPowerUp != null)
                    tripleShotPowerUp.SetActive(true);

                SetTextProcedural("Agarra el power-up de disparo triple y dispará con P mientras está activo.");
                break;

            case 5:
                SetTextProcedural("Tutorial completado. ¡Buena suerte!");
                break;
        }
    }

    private void SetTextProcedural(string text)
    {
        if (typeRoutine != null)
            StopCoroutine(typeRoutine);

        currentFullText = text;
        typeRoutine = StartCoroutine(TypeText());
    }

    private System.Collections.IEnumerator TypeText()
    {
        isTyping = true;
        if (tutorialText != null)
            tutorialText.text = "";

        foreach (char c in currentFullText)
        {
            tutorialText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;

        // Si es el último paso, cerrar al terminar de escribir
        if (stepIndex == 5)
            EndTutorial();
    }

    // Avanzar de paso SIEMPRE pasa por acá
    private void CompleteCurrentStep(float delay = 1f)
    {
        if (isTransitioning) return; // ya se está haciendo el cambio

        StartCoroutine(NextStepAfterDelay(delay));
    }

    private System.Collections.IEnumerator NextStepAfterDelay(float delay)
    {
        isTransitioning = true;
        yield return new WaitForSeconds(delay);

        stepIndex++;
        isTransitioning = false;

        StartStep();
    }

    // -------- PASO 1: WASD --------
    private void CheckMovement()
    {
        if (Input.GetKeyDown(KeyCode.W)) pressedW = true;
        if (Input.GetKeyDown(KeyCode.A)) pressedA = true;
        if (Input.GetKeyDown(KeyCode.S)) pressedS = true;
        if (Input.GetKeyDown(KeyCode.D)) pressedD = true;

        if (pressedW && pressedA && pressedS && pressedD)
            CompleteCurrentStep();
    }

    // -------- PASO 2: P --------
    private void CheckShoot()
    {
        if (Input.GetKeyDown(KeyCode.P))
            pressedShoot = true;

        if (pressedShoot)
            CompleteCurrentStep();
    }

    // -------- PASO 3: DASH --------
    private void CheckDash()
    {
        bool dir = Input.GetKey(KeyCode.W) ||
                   Input.GetKey(KeyCode.A) ||
                   Input.GetKey(KeyCode.S) ||
                   Input.GetKey(KeyCode.D);

        if (Input.GetKeyDown(KeyCode.Space) && dir)
            usedDash = true;

        if (usedDash)
            CompleteCurrentStep();
    }

    // -------- PASO 4: ALIADOS + F --------
    private void CheckAlliesAndF()
    {
        if (alliesBarSlider == null)
            return;

        if (alliesBarSlider.value >= requiredAlliesFill && Input.GetKeyDown(KeyCode.F))
        {
            alliesActivated = true;
        }

        if (alliesActivated)
            CompleteCurrentStep(6f);
    }

    // -------- PASO 5: POWER-UP TRIPLE + P --------
    private void CheckTripleShot()
    {
        if (projectileShoot == null)
            return;

        if (projectileShoot.IsTripleShotActive)
            triplePicked = true;

        if (triplePicked && projectileShoot.IsTripleShotActive && Input.GetKeyDown(KeyCode.P))
        {
            tripleUsed = true;
        }

        if (tripleUsed)
            CompleteCurrentStep(4f);
    }

    private void EndTutorial()
    {
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);

        if (GameManager.Instance != null)
            GameManager.Instance.OnTutorialCompleted();

        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();

        enabled = false;
    }
}
