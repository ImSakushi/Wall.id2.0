using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRadiusMin = 10f;
    public float spawnRadiusMax = 20f;
    public float spawnInterval = 2f;
    public LayerMask obstaclesLayer; // Layer pour les obstacles

    public int enemiesPerWave = 10; // Nombre d'ennemis à faire apparaître par vague
    private int spawnedEnemies = 0;
    private bool isSpawningEnabled = false; // Désactivé par défaut
    private bool isWaveActive = false; // Pour savoir si une vague est active

    private void Start()
    {
        // Plus de dépendance à vagueDisplayUI
    }

    void SpawnEnemy()
    {
        if (isSpawningEnabled && spawnedEnemies < enemiesPerWave)
        {
            Vector2 spawnPos = RandomCirclePosition();
            if (IsSpawnLocationFree(spawnPos))
            {
                Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                spawnedEnemies++;
            }
        }
    }

    Vector2 RandomCirclePosition()
    {
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        float radius = Random.Range(spawnRadiusMin, spawnRadiusMax);
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;
        return new Vector2(x, y);
    }

    bool IsSpawnLocationFree(Vector2 position)
    {
        float checkRadius = 1f; // Radius pour la vérification de collision
        Collider2D hit = Physics2D.OverlapCircle(position, checkRadius, obstaclesLayer);
        return hit == null; // Retourne vrai si aucun obstacle n'est détecté
    }

    void Update()
    {
        // Écouter l'entrée de la touche R
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isWaveActive)
            {
                StartWave();
            }
            else
            {
                EndWave();
            }
        }
    }

    void StartWave()
    {
        isWaveActive = true;
        isSpawningEnabled = true;
        spawnedEnemies = 0;

        // Commencer le spawn d'ennemis
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
        Debug.Log("Vague commencée !");
    }

    void EndWave()
    {
        isWaveActive = false;
        isSpawningEnabled = false;

        // Arrêter le spawn d'ennemis
        CancelInvoke("SpawnEnemy");
        Debug.Log("Vague terminée !");
    }
}
