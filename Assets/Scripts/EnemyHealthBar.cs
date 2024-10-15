using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image healthBarFill; // Référence à l'image de remplissage

    private EnemyHealth enemyHealth;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.OnHealthChanged += UpdateHealthBar;
            // Initialiser la barre de vie
            UpdateHealthBar(enemyHealth.CurrentHealth, enemyHealth.MaxHealth);
        }
        else
        {
            Debug.LogError("EnemyHealth script not found on the enemy.");
        }
    }

    private void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }

    // Méthode pour mettre à jour la barre de vie
    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthBarFill != null)
        {
            float fillAmount = Mathf.Clamp01((float)currentHealth / maxHealth);
            healthBarFill.fillAmount = fillAmount;
            Debug.Log($"Health Updated: {currentHealth}/{maxHealth} (Fill Amount: {fillAmount})");
        }
        else
        {
            Debug.LogError("HealthBarFill Image is not assigned in the Inspector.");
        }
    }
}
