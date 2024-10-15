using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void PlayGames()
  {
    // Charger la sc?ne de jeu
    UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
  }

  public void ExitGames()
  {
    // Quitter l'application
    Debug.Log("Quitting game...");
    Application.Quit();
  }
}
