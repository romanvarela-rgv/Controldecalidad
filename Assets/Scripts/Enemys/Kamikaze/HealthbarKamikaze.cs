using UnityEngine;
using UnityEngine.UI;

public class HealthbarKamikaze : MonoBehaviour
{
    [SerializeField] private Image healthBarImage;  // Referencia a la imagen de la barra de salud

    private void Update()
    {
        transform.rotation = Quaternion.identity;
        Vector3 newposition = transform.parent.position;
        newposition.y += 0.5f;
        transform.position = newposition;   
    }

    public void UpdateHealthbar(float maxHealth, float currentHealth)
    {

        healthBarImage.fillAmount = currentHealth / maxHealth;
    }
}
