using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
    public static AudioManager instance = null;

    public AudioSource m_soundStream;
    public AudioSource m_musicStream;

    // Ajout des variables pour l'Easter Egg
    [Header("Easter Egg Settings")]
    public AudioClip easterEggMusic; // Assignez cette musique dans l'Inspector
    public float easterEggPitch = 1.0f; // Pitch personnalisable pour l'Easter Egg

    // Définition du Konami Code
    private KeyCode[] konamiCode = new KeyCode[] {
        KeyCode.UpArrow,
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.DownArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.B,
        KeyCode.A,
        KeyCode.Return // Enter
    };

    private int konamiIndex = 0; // Index pour suivre la progression dans le Konami Code

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Update() {
        DetectKonamiCode();
    }

    // Méthode pour détecter le Konami Code
    private void DetectKonamiCode() {
        if (Input.GetKeyDown(konamiCode[konamiIndex])) {
            konamiIndex++;
            if (konamiIndex == konamiCode.Length) {
                TriggerEasterEgg();
                konamiIndex = 0;
            }
        }
        else if (Input.anyKeyDown) {
            // Si une touche incorrecte est pressée, réinitialiser l'index
            konamiIndex = 0;
        }
    }

    // Méthode pour déclencher l'Easter Egg
    private void TriggerEasterEgg() {
        if (easterEggMusic != null) {
            StopMusic(); // Arrêter la musique actuelle
            PlayMusic(easterEggMusic, true, m_musicStream.volume, easterEggPitch); // Jouer la musique de l'Easter Egg
            Debug.Log("Easter Egg activé ! Musique secrète en cours de lecture.");
        }
        else {
            Debug.LogWarning("Easter Egg Music n'est pas assignée dans l'Inspector.");
        }
    }

    public void PlaySound(AudioClip soundClipToPlay, float volume = 1.0f, float pitch = 1.0f) {
        m_soundStream.pitch = pitch;
        m_soundStream.volume = volume;
        m_soundStream.clip = soundClipToPlay;
        m_soundStream.Play();
    }

    public void StopSound() {
        m_soundStream.Stop();
    }

    public void PlayMusic(AudioClip musicClipToPlay, bool mustLoop, float volume = 1.0f, float pitch = 1.0f) {
        m_musicStream.pitch = pitch;
        m_musicStream.volume = volume;
        m_musicStream.loop = mustLoop;
        m_musicStream.clip = musicClipToPlay;
        m_musicStream.Play();
    }

    public void StopMusic() {
        m_musicStream.Stop();
    }
}
