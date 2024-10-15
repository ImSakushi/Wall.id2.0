using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Ajout de l'espace de noms pour la gestion des scènes

public class VagueDisplayUI : MonoBehaviour
{
  public Text timerText;
  private float countdownTime = 60f;
  public bool isNextWaveCountdown = true;
  private float timer;
  private int waveNumber = 1; // Commencer avec la vague 1

  public int[] enemiesPerWave = { 5, 7, 10, 12, 15 }; // Exemple : 5 ennemis pour la vague 1, 7 pour la vague 2, etc.

  void Start()
  {
    timer = countdownTime;
    UpdateTimerText();
  }

  void Update()
  {
    timer -= Time.deltaTime;

    if (timer <= 0)
    {
      isNextWaveCountdown = !isNextWaveCountdown;
      UpdateTimerText();

      if (isNextWaveCountdown) // Si c'est le début de la prochaine vague
      {
        waveNumber++; // Incrémenter le numéro de la vague
        CheckWinCondition(); // Vérifier si la condition de victoire est atteinte
      }

      timer = countdownTime;
    }
  }

  void UpdateTimerText()
  {
    if (isNextWaveCountdown)
    {
      timerText.text = "Prochaine vague dans : ";
    }
    else
    {
      timerText.text = "Vague " + waveNumber;
    }
  }

  public int GetEnemiesForCurrentWave()
  {
    if (waveNumber > 0 && waveNumber <= enemiesPerWave.Length)
    {
      return enemiesPerWave[waveNumber - 1];
    }
    else
    {
      return 0;
    }
  }

  private void CheckWinCondition()
  {
    // Si le joueur atteint la dernière vague
    if (waveNumber > enemiesPerWave.Length)
    {
      SceneManager.LoadScene("WinScene"); // Charger la scène de victoire
    }
  }
}
