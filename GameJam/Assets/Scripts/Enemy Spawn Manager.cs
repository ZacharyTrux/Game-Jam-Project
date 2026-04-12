using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnManager : MonoBehaviour {
    public static EnemySpawnManager Instance { get; private set; }
    public List<GameObject> enemies = new();
    public bool spawnEnabled = true;
    public float spawnRate = 1f;
    public int maxEnemies = 2;

    public float minDistance = 5f;
    public float maxDistance = 15f;
    public float spread = 5f;
    private float spawnTimer = 0f;
    private int enemyCount = 0;

    private Camera camera;


    void Awake(){
        if(Instance != null){
            Destroy(gameObject);
            return; 
        }
        Instance = this;
        DontDestroyOnLoad(Instance);
    }

    void Start(){
        camera = Camera.main;
    }

    void Update(){
        if(Time.time > spawnTimer){
            SpawnRandomEnemy();
            spawnTimer = Time.time + (1f / spawnRate);
        }
    }

    private void SpawnRandomEnemy(){
        if(!spawnEnabled) return;
        if(enemyCount >= maxEnemies) return;

        // For simplicity, spawn at random position around player
        Vector3 randomSpawnPosition = GetRandomPositionBehindCamera();
        GameObject enemyPrefab = getRandomEnemy();
        if(enemyPrefab != null){
            Instantiate(enemyPrefab, randomSpawnPosition, Quaternion.identity);
            enemyCount++;
        }
    }

    private Vector3 GetRandomPositionBehindCamera(){
        float height = 2f * camera.orthographicSize;
        float width = height * camera.aspect;

        // 2. Define the "Safe Zone" (the area the player sees)
        // We want to spawn outside of: width/2 and height/2
        float xThreshold = width / 2f;
        float yThreshold = height / 2f;

        // 3. Pick a random direction (anywhere in 360 degrees)
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        // 4. Calculate spawn position
        // We take the threshold and add our minDistance to ensure it's off-screen
        float spawnDistX = xThreshold + Random.Range(minDistance, maxDistance);
        float spawnDistY = yThreshold + Random.Range(minDistance, maxDistance);

        Vector3 spawnPos = new Vector3(
            camera.transform.position.x + (direction.x * spawnDistX),
            camera.transform.position.y + (direction.y * spawnDistY),
            0f // Keep Z at 0 for 2D
        );

        return spawnPos;
    }
    private GameObject getRandomEnemy(){
        if(enemies.Count == 0){
          return null;   
        }
        
        int index = Random.Range(0, enemies.Count);
        return enemies[index];
    }

    private void SpawnEnemy(Enemy enemyPrefab, Vector3 position){
        Instantiate(enemyPrefab, position, Quaternion.identity);
    }

    public void IncreaseStats(float healthPoolIncrease, float spawnRateMultiplier){
        spawnRate *= spawnRateMultiplier;
        foreach(GameObject enemy in enemies){
            var enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.health += enemyScript.health * healthPoolIncrease;
        }
    }
}