using UnityEngine;

public class PointToZero : MonoBehaviour
{
    public Transform player; // Référence à la transform du joueur

    void Update()
    {
        if (player != null)
        {
            Vector3 dirToZero = Vector3.zero - player.position;
            float angle = Mathf.Atan2(dirToZero.y, dirToZero.x) * Mathf.Rad2Deg;

            // Ajustez l'angle initial de la flèche pour qu'elle pointe dans la bonne direction
            float initialAngle = 90f; // Ajustez cet angle selon la rotation initiale de la flèche
            Quaternion targetRotation = Quaternion.AngleAxis(angle + initialAngle, Vector3.forward);

            transform.rotation = targetRotation;
        }
    }
}
