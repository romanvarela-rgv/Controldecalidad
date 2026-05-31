using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormManager : MonoBehaviour
{
    public List<GameObject> lightningStrikes; // Lista de rayos
    public GameObject shadowPrefab; // Prefab de la sombra
    [SerializeField] private float minTimeBetweenStrikes; // Tiempo mínimo entre rayos
    [SerializeField] private float maxTimeBetweenStrikes; // Tiempo máximo entre rayos
    [SerializeField] private float lightningDuration; // Duración de un rayo
    [SerializeField] private float shadowWarningDuration = 1.5f; // Tiempo que la sombra está activa antes del rayo
    [SerializeField] private int maxSimultaneousStrikes = 3; // Número de rayos a activar simultáneamente
    [SerializeField] private GameObject warningSprite; // Advertencia visual (puede ser un sprite o UI)
    [SerializeField] private float warningDuration = 2f; // Duración de la advertencia

    private bool isStormActive = false; // Variable para verificar si la tormenta está activa

    private void Start()
    {
        if (warningSprite != null)
        {
            warningSprite.SetActive(false);
        }
    }

    public void StartStorm()
    {
        if (!isStormActive)
        {
            isStormActive = true;
            StartCoroutine(StormWarningRoutine());
        }
    }

    public void StopStorm()
    {
        isStormActive = false;
        StopAllCoroutines();

        // Desactivar todos los rayos y la advertencia cuando la tormenta termine
        foreach (var lightning in lightningStrikes)
        {
            lightning.SetActive(false);
        }

        if (warningSprite != null)
        {
            warningSprite.SetActive(false);
        }
    }

    private IEnumerator StormWarningRoutine()
    {
        // Mostrar advertencia
        if (warningSprite != null)
        {
            warningSprite.SetActive(true);
        }

        // Esperar durante la advertencia
        yield return new WaitForSeconds(warningDuration);

        // Ocultar advertencia
        if (warningSprite != null)
        {
            warningSprite.SetActive(false);
        }

        // Comenzar la tormenta
        StartCoroutine(StormRoutine());
    }

    private IEnumerator StormRoutine()
    {
        while (isStormActive)
        {
            // Determina cuántos rayos se activarán en este ciclo
            int strikesToActivate = Random.Range(1, maxSimultaneousStrikes + 1);

            List<GameObject> selectedLightningStrikes = new List<GameObject>();

            for (int i = 0; i < strikesToActivate; i++)
            {
                GameObject selectedLightning;

                // Seleccionar un rayo no repetido
                do
                {
                    selectedLightning = lightningStrikes[Random.Range(0, lightningStrikes.Count)];
                } while (selectedLightningStrikes.Contains(selectedLightning));

                selectedLightningStrikes.Add(selectedLightning);

                // Instanciar sombra en la posición del rayo
                if (shadowPrefab != null)
                {
                    GameObject shadow = Instantiate(shadowPrefab, selectedLightning.transform.position, Quaternion.identity);
                    Destroy(shadow, shadowWarningDuration); // Destruir la sombra después de su duración
                }
            }

            // Esperar el tiempo de advertencia de sombra
            yield return new WaitForSeconds(shadowWarningDuration);

            // Activar los rayos seleccionados
            foreach (var lightning in selectedLightningStrikes)
            {
                lightning.SetActive(true);
            }

            // Esperar el tiempo de duración de los rayos
            yield return new WaitForSeconds(lightningDuration);

            // Desactivar todos los rayos
            foreach (var lightning in lightningStrikes)
            {
                lightning.SetActive(false);
            }

            // Esperar entre strikes antes de activar otro ciclo
            yield return new WaitForSeconds(Random.Range(minTimeBetweenStrikes, maxTimeBetweenStrikes));
        }
    }
}
