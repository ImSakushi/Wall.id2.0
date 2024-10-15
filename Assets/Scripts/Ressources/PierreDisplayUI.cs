using UnityEngine;
using UnityEngine.UI;

public class PierreDisplayUI : MonoBehaviour
{
  public Text pierreText;

  public int nombrePierre = 0;

  void Start()
  {
    UpdateStoneDisplay();
  }

  // Cette méthode met à jour l'affichage de la pierre
  public void UpdateStoneDisplay()
  {
    pierreText.text = " " + nombrePierre.ToString();
  }
}
