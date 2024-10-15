using UnityEngine;
using UnityEngine.UI;

public class BoisDisplayUI : MonoBehaviour
{
  public Text boisText;

  public int nombreBois = 0;

  void Start()
  {
    UpdateWoodDisplay();
  }

  // Cette méthode met à jour l'affichage du bois
  public void UpdateWoodDisplay()
  {
    boisText.text = " " + nombreBois.ToString();
  }
}
