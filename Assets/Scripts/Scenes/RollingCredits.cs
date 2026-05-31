using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RollingCredits : MonoBehaviour
{
    public RectTransform panelTransform;
    public Vector2 targetPosition;
    public float moveDuration = 11f;

    void Start()
    {
        StartCoroutine(RollCredits());
    }

    IEnumerator RollCredits()
    {
        float elapsedTime = 0f;
        Vector2 initialPosition = panelTransform.anchoredPosition;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            panelTransform.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        panelTransform.anchoredPosition = targetPosition;

        CreditsEnd();
    }

    void CreditsEnd()
    {
        SceneManager.LoadScene(0);
    }
}
