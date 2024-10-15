using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
  public float damageRate = 2f; // Délai entre chaque dommage
  public float damageAmount = 1f; // Montant de dégâts à infliger à chaque intervalle
  private float nextDamageTime = 0f; // Temps pour le prochain dommage

  private Health plantHealth; // Référence au script Health de la plante
  private bool canDealDamage = false; // Indique si l'ennemi peut infliger des dégâts

  private void Start()
  {
    // Obtenez la référence au script Health de la plante
    plantHealth = GameObject.FindGameObjectWithTag("Plant").GetComponent<Health>();
  }

  private void Update()
  {
    // Vérifiez si l'ennemi peut infliger des dégâts
    if (canDealDamage && Time.time >= nextDamageTime)
    {
      DealDamage();
      // Mettre à jour le temps du prochain dommage
      nextDamageTime = Time.time + damageRate;
    }
  }

  private void OnCollisionEnter2D(Collision2D collision)
  {
    if (collision.gameObject.CompareTag("Plant") && !canDealDamage)
    {
      // Activation des dégâts continus après la première collision
      canDealDamage = true;
    }
  }

  private void DealDamage()
  {
    // Réduire la santé de la plante de damageAmount
    plantHealth.AdjustHealth(-damageAmount);
    Debug.Log("La plante subit " + damageAmount + " points de dégât.");
  }
}
