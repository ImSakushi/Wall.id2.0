using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class MessageLoader : MonoBehaviour
{
  public GameObject signPrefab; // Référence au préfab 'Sign'

  void Start()
  {
    StartCoroutine(GetMessagesFromServer());
  }

  IEnumerator GetMessagesFromServer()
  {
    string url = "https://wallidgame.000webhostapp.com/get_message.php";
    using (UnityWebRequest www = UnityWebRequest.Get(url))
    {
      yield return www.SendWebRequest();

      if (www.result != UnityWebRequest.Result.Success)
      {
        Debug.LogError("Erreur de chargement : " + www.error);
      }
      else
      {
        ProcessMessages(www.downloadHandler.text);
      }
    }
  }

  void ProcessMessages(string data)
  {
    string[] messages = data.Split(';');
    foreach (string messageData in messages)
    {
      if (string.IsNullOrEmpty(messageData)) continue;

      string[] fields = messageData.Split(',');
      if (fields.Length >= 3)
      {
        string messageText = fields[0];
        float positionX = float.Parse(fields[1]);
        float positionY = float.Parse(fields[2]);

        Vector3 position = new Vector3(positionX, positionY, 0); // Z position is set to 0
        GameObject newSign = Instantiate(signPrefab, position, Quaternion.identity);

        Dialog signDialog = newSign.GetComponentInChildren<Dialog>();
        if (signDialog != null)
        {
          DialogPage newPage = new DialogPage { text = "\"" + messageText + "\" - signé un ancêtre Robot", color = Color.black };
          signDialog.m_dialogWithPlayer.Add(newPage);
        }
      }
    }
  }

}
