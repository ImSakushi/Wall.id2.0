using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    [Header("Sprites for Directions")]
    public Sprite spriteNorth;       // Sprite pour la direction Nord
    public Sprite spriteSouth;       // Sprite pour la direction Sud
    public Sprite spriteEast;        // Sprite pour la direction Est
    public Sprite spriteWest;        // Sprite pour la direction Ouest
    public Sprite spriteNorthEast;   // Sprite pour la direction Nord-Est
    public Sprite spriteNorthWest;   // Sprite pour la direction Nord-Ouest
    public Sprite spriteSouthEast;   // Sprite pour la direction Sud-Est
    public Sprite spriteSouthWest;   // Sprite pour la direction Sud-Ouest

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Définit la direction de la tourelle en fonction du vecteur de direction fourni.
    /// </summary>
    /// <param name="direction">Vecteur de direction.</param>
    public void SetDirection(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            Debug.LogWarning("Direction nulle reçue pour la tourelle.");
            return;
        }

        // Normaliser la direction pour éviter les erreurs
        direction = direction.normalized;

        // Réinitialiser la rotation
        transform.rotation = Quaternion.identity;

        // Déterminer l'angle en degrés
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Appliquer la rotation en fonction de l'angle
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // Ajuster selon votre orientation

        // Déterminer quel sprite utiliser en fonction de la direction
        if (direction == Vector2.up)
        {
            spriteRenderer.sprite = spriteNorth;
        }
        else if (direction == Vector2.down)
        {
            spriteRenderer.sprite = spriteSouth;
        }
        else if (direction == Vector2.right)
        {
            spriteRenderer.sprite = spriteEast;
        }
        else if (direction == Vector2.left)
        {
            spriteRenderer.sprite = spriteWest;
        }
        else if (direction.x > 0 && direction.y > 0)
        {
            spriteRenderer.sprite = spriteNorthEast;
        }
        else if (direction.x < 0 && direction.y > 0)
        {
            spriteRenderer.sprite = spriteNorthWest;
        }
        else if (direction.x > 0 && direction.y < 0)
        {
            spriteRenderer.sprite = spriteSouthEast;
        }
        else if (direction.x < 0 && direction.y < 0)
        {
            spriteRenderer.sprite = spriteSouthWest;
        }
        else
        {
            // Valeur par défaut si aucune condition n'est remplie
            spriteRenderer.sprite = spriteSouth;
        }
    }
}
