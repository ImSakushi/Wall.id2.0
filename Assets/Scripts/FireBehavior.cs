using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBehavior : MonoBehaviour
{
    public AudioClip launchSound;
    private AudioSource audioSource;
    Rigidbody2D m_rb2D;

    float m_launchedTime;
    public float fireDuration = 2f; // Rendre public pour ajustement via l'inspecteur
    public float speed = 100f; // Rendre public pour ajustement via l'inspecteur
    public int damage = 2; // Nouvelle variable pour les dégâts

    void Awake()
    {
        m_rb2D = gameObject.GetComponent<Rigidbody2D>();
        m_launchedTime = Time.realtimeSinceStartup;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = launchSound;
    }

    void Start()
    {
        gameObject.tag = "Fireball";
        // Jouer le son de lancement
        if (launchSound != null)
        {
            audioSource.Play();
        }
    }

    public void Launch(Vector2 direction)
    {
        m_rb2D.AddForce(direction.normalized * speed, ForceMode2D.Impulse);
    }

    void Update()
    {
        if (Time.realtimeSinceStartup > m_launchedTime + fireDuration)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fireball") || collision.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
        else if (collision.gameObject.CompareTag("Fireball") || collision.gameObject.CompareTag("Farmable") || collision.gameObject.CompareTag("Plant"))
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage); // Utiliser la variable damage
                Destroy(gameObject);
            }
        }
    }
}
