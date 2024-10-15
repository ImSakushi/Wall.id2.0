using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowBehavior : MonoBehaviour
{
    [Header("Glow Settings")]
    public float glowRadius = 1.5f; // Rayon maximal de la lueur
    public float squashSpeed = 5f; // Vitesse de lissage du squash
    public LayerMask wallLayer; // Layer des murs pour la détection

    private Transform playerTransform;
    private Camera mainCamera;
    private Vector3 targetPosition;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 originalScale;

    void Start()
    {
        playerTransform = transform.parent; // Assure que le Glow est enfant du Player
        mainCamera = Camera.main;
        originalScale = transform.localScale;
    }

    void Update()
    {
        UpdateGlowPosition();
    }

    private void UpdateGlowPosition()
    {
        if (playerTransform == null || mainCamera == null)
            return;

        // Obtenir la position de la souris en coordonnées du monde
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // S'assurer que z = 0 pour un jeu 2D

        // Calculer la direction du joueur vers la souris
        Vector2 direction = (mousePosition - playerTransform.position).normalized;

        // Calculer la position cible basée sur la direction et le rayon
        Vector3 desiredPosition = (Vector3)direction * glowRadius;

        // Effectuer un raycast pour détecter les murs
        RaycastHit2D hit = Physics2D.Raycast(playerTransform.position, direction, glowRadius, wallLayer);

        if (hit.collider != null)
        {
            // Si un mur est détecté, ajuster la position cible
            float distance = hit.distance - 0.1f; // Soustraire une petite marge
            desiredPosition = (Vector3)direction * distance;

            // Calculer le facteur de squash basé sur la proximité
            float squashFactor = Mathf.Clamp01(1 - (distance / glowRadius));

            // Appliquer le squash sur l'échelle
            Vector3 newScale = originalScale;
            newScale.x = Mathf.Lerp(originalScale.x, originalScale.x * (1 - 0.5f * squashFactor), Time.deltaTime * squashSpeed);
            newScale.y = Mathf.Lerp(originalScale.y, originalScale.y * (1 + 0.5f * squashFactor), Time.deltaTime * squashSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale, newScale, Time.deltaTime * squashSpeed);
        }
        else
        {
            // Si aucun mur n'est détecté, positionner normalement et rétablir l'échelle originale
            desiredPosition = (Vector3)direction * glowRadius;
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * squashSpeed);
        }

        // Lisser le mouvement de la lueur
        targetPosition = desiredPosition;
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref currentVelocity, 1f / squashSpeed);
    }

    // Debugging : Visualiser le raycast dans l'éditeur
    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null)
            return;

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector2 direction = (mousePosition - playerTransform.position).normalized;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(playerTransform.position, playerTransform.position + (Vector3)direction * glowRadius);
    }
}
