using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehavior : MonoBehaviour
{
  public GameObject projectilePrefab; // Le préfab de projectile à tirer
  public float fireRate = 2f; // La cadence de tir (tirs par seconde)
  public float firingRange = 10f;
  public float firingAngle = 45f; // Angle de tir en degrés
  private float nextFireTime = 0f;

  void Update()
  {
    if (Time.time >= nextFireTime)
    {
      FireAtNearestEnemy();
      nextFireTime = Time.time + 1f / fireRate;
    }
  }

  void FireAtNearestEnemy()
  {
    GameObject nearestEnemy = FindNearestEnemy();

    if (nearestEnemy != null)
    {
      Vector2 directionToEnemy = (nearestEnemy.transform.position - transform.position).normalized;
      if (IsEnemyInFiringAngle(directionToEnemy))
      {
        FireProjectile(directionToEnemy);
      }
    }
  }

  GameObject FindNearestEnemy()
  {
    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
    GameObject nearestEnemy = null;
    float minDistance = Mathf.Infinity;

    foreach (GameObject enemy in enemies)
    {
      float distance = Vector2.Distance(transform.position, enemy.transform.position);
      if (distance < minDistance && distance <= firingRange)
      {
        nearestEnemy = enemy;
        minDistance = distance;
      }
    }

    return nearestEnemy;
  }

  bool IsEnemyInFiringAngle(Vector2 directionToEnemy)
  {
    float angleToEnemy = Vector2.Angle(transform.right, directionToEnemy);
    return angleToEnemy <= firingAngle;
  }

  void FireProjectile(Vector2 direction)
  {
    GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

    FireBehavior fireBehavior = projectile.GetComponent<FireBehavior>();
    BulletBehavior bulletBehavior = projectile.GetComponent<BulletBehavior>();

    if (fireBehavior != null)
    {
      fireBehavior.Launch(direction);
    }

    if (bulletBehavior != null)
    {
      bulletBehavior.Launch(direction);
    }
  }

}
