/* Author : Raphaël Marczak - 2018/2020, for MIAMI Teaching (IUT Tarbes) and MMI Teaching (IUT Bordeaux Montaigne)
 *
 * This work is licensed under the CC0 License.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// This struct represents one dialog page
// (text on the current page, and its color)
[System.Serializable]

public struct DialogPage
{
  public string text;
  public Color color;
}

// This class is used to correctly display a full dialog
public class DialogManager : MonoBehaviour
{

  public Text m_renderText;
  private List<DialogPage> m_dialogToDisplay;
  public float typingSpeed = 0.05f; // Vitesse à laquelle les lettres apparaissent
  private string currentText = ""; // Texte actuellement affiché
  private bool isTyping = false; // Si le texte est en train d'être tapé
  private int currentPageIndex = 0; // Index de la page de dialogue actuelle
  public AudioClip typingSound; // Le son joué à chaque lettre
  private AudioSource audioSource;


  void Awake()
  {
    audioSource = gameObject.AddComponent<AudioSource>();
    audioSource.playOnAwake = false;
    audioSource.clip = typingSound;
  }

  // Sets the dialog to be displayed
  public void SetDialog(List<DialogPage> dialogToAdd)
  {
    m_dialogToDisplay = new List<DialogPage>(dialogToAdd);
    currentPageIndex = 0;

    if (m_dialogToDisplay.Count > 0)
    {
      if (m_renderText != null)
      {
        // Assurez-vous que l'objet de jeu est actif avant de lancer la coroutine
        this.gameObject.SetActive(true);

        m_renderText.text = "";
        StartCoroutine(TypeText(m_dialogToDisplay[currentPageIndex].text));
      }
    }
    else
    {
      // Désactivez l'objet de jeu si la liste de dialogues est vide
      this.gameObject.SetActive(false);
    }
  }



  IEnumerator TypeText(string textToType)
  {
    isTyping = true;
    currentText = "";

    foreach (char letter in textToType.ToCharArray())
    {
      currentText += letter;
      m_renderText.text = currentText;
      audioSource.Play(); // Jouer le son à chaque lettre
      yield return new WaitForSeconds(typingSpeed);
    }

    isTyping = false;
  }




  // Update is called once per frame
  void Update()
  {
    if (m_renderText == null)
    {
      this.gameObject.SetActive(false);
      return;
    }

    if (m_dialogToDisplay.Count > 0 && !isTyping)
    {
      if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z))
      {
        currentPageIndex++;

        if (currentPageIndex < m_dialogToDisplay.Count)
        {
          StartCoroutine(TypeText(m_dialogToDisplay[currentPageIndex].text));
        }
        else
        {
          this.gameObject.SetActive(false);
        }
      }
    }

    // Permettre au joueur de passer directement au texte complet
    if (Input.GetKeyDown(KeyCode.X) && isTyping)
    {
      StopAllCoroutines();
      isTyping = false;
      m_renderText.text = m_dialogToDisplay[currentPageIndex].text;
    }
  }


  public bool IsOnScreen()
  {
    return this.gameObject.activeSelf;
  }
}
