using UnityEngine;
using UnityEngine.UI;

public class MetalDisplayUI : MonoBehaviour
{
  public Text metalText;

  public int nombreMetal = 0;

  void Start()
  {
    UpdateIronDisplay();
  }

  // Cette méthode met à jour l'affichage du metal
  public void UpdateIronDisplay()
  {
    metalText.text = " " + nombreMetal.ToString();
  }
}
