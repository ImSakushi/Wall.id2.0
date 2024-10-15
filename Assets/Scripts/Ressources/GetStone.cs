using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetStone : MonoBehaviour
{
  public PierreDisplayUI PierreReference;
  public AudioClip stoneCollectSound; // Son à jouer lors de la récolte de la pierre
  [Range(0, 1)]
  public float volume = 0.5f; // Volume du son, réglable de 0 (silence) à 1 (volume maximal)

  private void OnCollisionEnter2D(Collision2D collision)
  {
    // Vérifier si le joueur entre en collision avec cet objet
    if (collision.gameObject.CompareTag("Player"))
    {
      PlayStoneCollectSound();
      PierreReference.nombrePierre++; // Ajouter de la pierre
      PierreReference.UpdateStoneDisplay(); // Mettre à jour l'affichage de la pierre
      Destroy(gameObject);
    }
  }

  private void PlayStoneCollectSound()
  {
    if (stoneCollectSound != null)
    {
      // Créer un nouvel objet de jeu pour jouer le son
      GameObject tempAudioSource = new GameObject("TempAudio");
      AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
      audioSource.clip = stoneCollectSound;
      audioSource.volume = volume;
      audioSource.Play();

      // Détruire l'objet temporaire une fois le son terminé
      Destroy(tempAudioSource, stoneCollectSound.length);
    }
  }
}
