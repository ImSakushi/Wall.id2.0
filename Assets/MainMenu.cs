using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void PlayGame()
  {
    // Charger la sc?ne de jeu
    UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
  }

  public void Credits()
  {
    // Charger la sc?ne de jeu
    UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
  }

  public void Menu()
  {
    // Charger la sc?ne de jeu
    UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
  }

  public void ExitGame()
  {
    // Quitter l'application
    Debug.Log("Quitting game...");
    Application.Quit();
  }
}
