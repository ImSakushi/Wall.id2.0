using UnityEngine;
using Cinemachine;
using System;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public AudioClip damageSound;
    public AudioClip deathSound;
    private AudioSource audioSource;

    private int currentHealth;

    public GameObject explosionPrefab;

    // Déclarer un événement pour les changements de santé
    public event Action<int, int> OnHealthChanged;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        // Si aucun AudioSource n'est attaché à l'ennemi, ajoutez-en un.
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Notifier l'initialisation de la barre de vie
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        // Jouer le son de dégâts
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        // Notifier le changement de santé
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        CinemachineShake.Instance.ShakeCamera(15f, .25f);
        // Jouer le son de mort sur un nouvel objet de jeu temporaire
        if (deathSound != null)
        {
            GameObject audioPlayer = new GameObject("DeathSoundPlayer");
            AudioSource audioSource = audioPlayer.AddComponent<AudioSource>();
            audioSource.clip = deathSound;
            audioSource.volume = 0.6f; // Réglez le volume selon vos besoins
            audioSource.Play();

            // Détruire l'objet de jeu une fois le son terminé
            Destroy(audioPlayer, deathSound.length);
        }

        // Instancier l'explosion au moment de la mort
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Détruire l'ennemi immédiatement
        Destroy(gameObject);
    }
}
