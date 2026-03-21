using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRate = 3f;
    public float spawnDistance = 15f;

    private float _spawnTimer;

    void Update()
    {
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0)
        {
            SpawnEnemy();
            _spawnTimer = spawnRate;
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        Vector2 spawnPos = Random.insideUnitCircle.normalized * spawnDistance;
        Instantiate(enemyPrefab, (Vector3)spawnPos, Quaternion.identity);
    }
}
