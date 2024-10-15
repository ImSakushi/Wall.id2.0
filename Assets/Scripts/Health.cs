using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Health : MonoBehaviour
{
  public Text healthText;
  public Image redScreenImage; // Référence à l'Image de l'écran rouge
  [SerializeField]
  public float healthPlant = 100f;

  private float lastHealth;

  void Start()
  {
    UpdateHealthText();
    lastHealth = healthPlant;
  }

  void Update()
  {
    UpdateHealthText();
    CheckGameOver();
    CheckHealthChange();
  }

  void UpdateHealthText()
  {
    healthText.text = Mathf.Clamp(healthPlant, 0f, 100f).ToString() + "/100";
  }

  // Fonction pour ajuster la santé de la plante
  public void AdjustHealth(float amount)
  {
    healthPlant = Mathf.Clamp(healthPlant + amount, 0f, 100f);
    UpdateHealthText();
  }

  private void CheckGameOver()
  {
    if (healthPlant <= 0)
    {
      SceneManager.LoadScene("GameOverScene");
    }
  }

  private void CheckHealthChange()
  {
    if (healthPlant < lastHealth - 4f)
    {
      StartCoroutine(ShowRedScreen());
      lastHealth = healthPlant;
    }
  }

  IEnumerator ShowRedScreen()
  {
    redScreenImage.gameObject.SetActive(true);
    float elapsed = 0f;
    float duration = 2f;

    while (elapsed < duration)
    {
      elapsed += Time.deltaTime;
      float alpha = Mathf.Lerp(0.75f, 0f, elapsed / duration);
      redScreenImage.color = new Color(0.75f, 0f, 0f, alpha);
      yield return null;
    }

    redScreenImage.gameObject.SetActive(false);
  }
}
