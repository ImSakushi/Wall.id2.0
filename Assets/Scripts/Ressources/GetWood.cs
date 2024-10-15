using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWood : MonoBehaviour
{
  public BoisDisplayUI BoisReference;
  public AudioClip woodCollectSound; // Son à jouer lors de la récolte du bois
  [Range(0, 1)]
  public float volume = 0.5f; // Volume du son, réglable de 0 (silence) à 1 (volume maximal)

  private void OnCollisionEnter2D(Collision2D collision)
  {
    // Vérifier si le joueur entre en collision avec cet objet
    if (collision.gameObject.CompareTag("Player"))
    {
      PlayWoodCollectSound();
      BoisReference.nombreBois++; // Ajouter du bois
      BoisReference.UpdateWoodDisplay(); // Mettre à jour l'affichage du bois
      Destroy(gameObject);
    }
  }

  private void PlayWoodCollectSound()
  {
    if (woodCollectSound != null)
    {
      // Créer un nouvel objet de jeu pour jouer le son
      GameObject tempAudioSource = new GameObject("TempAudio");
      AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
      audioSource.clip = woodCollectSound;
      audioSource.volume = volume;
      audioSource.Play();

      // Détruire l'objet temporaire une fois le son terminé
      Destroy(tempAudioSource, woodCollectSound.length);
    }
  }
}
