using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetIron : MonoBehaviour
{
  public MetalDisplayUI MetalReference;
  public AudioClip ironCollectSound; // Son à jouer lors de la récolte du fer
  [Range(0, 1)]
  public float volume = 0.5f; // Volume du son, réglable de 0 (silence) à 1 (volume maximal)

  // Update is called once per frame
  void Update()
  {
    // ...
  }

  private void OnCollisionEnter2D(Collision2D collision)
  {
    // Vérifier si le joueur entre en collision avec cet objet
    if (collision.gameObject.CompareTag("Player"))
    {
      PlayIronCollectSound();
      MetalReference.nombreMetal++; // Ajouter du fer
      MetalReference.UpdateIronDisplay(); // Mettre à jour l'affichage du fer
      Destroy(gameObject);
    }
  }

  private void PlayIronCollectSound()
  {
    if (ironCollectSound != null)
    {
      // Créer un nouvel objet de jeu pour jouer le son
      GameObject tempAudioSource = new GameObject("TempAudio");
      AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
      audioSource.clip = ironCollectSound;
      audioSource.volume = volume;
      audioSource.Play();

      // Détruire l'objet temporaire une fois le son terminé
      Destroy(tempAudioSource, ironCollectSound.length);
    }
  }
}
