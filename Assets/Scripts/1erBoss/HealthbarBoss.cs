using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarBoss : MonoBehaviour
{
    [SerializeField] private Image healthbar;

    public void UpdateHealthbar(float maxHealth, float health)
    {
        healthbar.fillAmount = health / maxHealth;
    }

}
