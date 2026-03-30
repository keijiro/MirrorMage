using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Pool Settings")]
    [Tooltip("List of enemy prefabs to spawn randomly")]
    public GameObject[] enemyPrefabs;
    
    [Header("Spawn Frequency")]
    public float baseSpawnsPerMinute = 20f;
    public float spawnsIncreasePerXP = 0.05f;

    [Header("Enemy Limit")]
    public int baseMaxEnemies = 10;
    public float maxEnemiesIncreasePerXP = 0.02f;

    [Header("Placement")]
    [Tooltip("Distance outside the screen where enemies will spawn")]
    public float spawnMargin = 1f;
    [Tooltip("Minimum distance from the player to spawn an enemy")]
    public float minPlayerDistance = 5f;

    private float _spawnTimer;
    private Camera _cam;
    private PlayerController _player;

    void Start()
    {
        _cam = Camera.main;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.GetComponent<PlayerController>();
        }
        
        // Initialize timer for first spawn
        _spawnTimer = 0f;
    }

    void Update()
    {
        if (_player == null) return;

        _spawnTimer -= Time.deltaTime;
        
        if (_spawnTimer <= 0)
        {
            // Calculate current limits based on XP
            float xp = _player.totalXP;
            float currentSpawnsPerMin = baseSpawnsPerMinute + (xp * spawnsIncreasePerXP);
            int currentMaxEnemies = Mathf.FloorToInt(baseMaxEnemies + (xp * maxEnemiesIncreasePerXP));

            // Count current enemies
            int activeEnemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length;

            if (activeEnemies < currentMaxEnemies)
            {
                SpawnEnemy();
            }

            // Calculate next spawn interval (60s / count per minute)
            _spawnTimer = 60f / Mathf.Max(1f, currentSpawnsPerMin);
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0 || _cam == null) return;

        // Pick a random prefab from the list
        GameObject selectedPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        if (selectedPrefab == null) return;

        Vector3 spawnPos = GetRandomSpawnPosition();

        // Ensure we don't spawn too close to the player
        if (_player != null)
        {
            int maxRetries = 10;
            while (Vector2.Distance(spawnPos, _player.transform.position) < minPlayerDistance && maxRetries > 0)
            {
                spawnPos = GetRandomSpawnPosition();
                maxRetries--;
            }
        }

        Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // Get camera bounds in world space
        float height = 2f * _cam.orthographicSize;
        float width = height * _cam.aspect;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        // Calculate the rectangle just outside the screen
        float xMax = halfWidth + spawnMargin;
        float yMax = halfHeight + spawnMargin;

        Vector3 camPos = _cam.transform.position;
        
        // Randomly pick one of the four sides
        int side = Random.Range(0, 4);
        float x = 0, y = 0;

        switch (side)
        {
            case 0: // Top
                x = Random.Range(-xMax, xMax);
                y = yMax;
                break;
            case 1: // Bottom
                x = Random.Range(-xMax, xMax);
                y = -yMax;
                break;
            case 2: // Left
                x = -xMax;
                y = Random.Range(-yMax, yMax);
                break;
            case 3: // Right
                x = xMax;
                y = Random.Range(-yMax, yMax);
                break;
        }

        return new Vector3(camPos.x + x, camPos.y + y, 0f);
    }
}
