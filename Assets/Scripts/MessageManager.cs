using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class MessageManager : MonoBehaviour
{
  public TMP_InputField messageInput;
  public Button submitButton;
  public GameObject signPanel; // Panneau pour entrer le message
  public GameObject signPrefab; // Préfab 'Sign' à instancier
  public Transform playerTransform; // Référence à la position du joueur

  private string url = "https://wallidgame.000webhostapp.com/submit_message.php"; // URL du script PHP

  void Start()
  {
    submitButton.onClick.AddListener(SubmitMessage);
  }

  void SubmitMessage()
  {
    string message = messageInput.text;

    if (!string.IsNullOrEmpty(message))
    {
      // Récupérer la position actuelle du joueur
      Vector3 position = playerTransform.position;
      StartCoroutine(SendDataToServer(message, position));

      // Instancier le préfab 'Sign' et mettre à jour son dialogue
      GameObject newSign = Instantiate(signPrefab, position, Quaternion.identity);
      Dialog signDialog = newSign.GetComponentInChildren<Dialog>();

      if (signDialog != null)
      {
        // Formatage du message pour inclure des guillemets et une signature
        string formattedMessage = "\"" + message + "\" - signé un ancêtre Robot";
        DialogPage newPage = new DialogPage { text = formattedMessage, color = Color.black };
        signDialog.m_dialogWithPlayer.Add(newPage);
      }

      // Réinitialiser l'InputField et cacher le panneau
      messageInput.text = "";
      signPanel.SetActive(false);
    }
    else
    {
      Debug.Log("Le message est vide.");
    }
  }



  IEnumerator SendDataToServer(string message, Vector3 position)
  {
    WWWForm form = new WWWForm();
    form.AddField("message", message);
    form.AddField("positionX", position.x.ToString());
    form.AddField("positionY", position.y.ToString());
    form.AddField("positionZ", position.z.ToString());

    using (UnityWebRequest www = UnityWebRequest.Post(url, form))
    {
      yield return www.SendWebRequest();

      if (www.result != UnityWebRequest.Result.Success)
      {
        Debug.Log(www.error);
      }
      else
      {
        Debug.Log("Message envoyé : " + www.downloadHandler.text);
      }
    }
  }
}
