using UnityEngine;
using System.Collections.Generic;
using System;



public class EnemySpawnManager : MonoBehaviour {
    [System.Serializable]
    public class EnemyWeightData{
        public GameObject prefab;
        public float weight;
    }
    public static EnemySpawnManager Instance { get; private set; }
    public List<EnemyWeightData> enemiesData = new();
    private Dictionary<GameObject, float> enemies = new();
    public bool spawnEnabled = true;
    public float spawnRate = 1f;
    public int maxEnemies = 2;

    public float minDistance = 5f;
    public float maxDistance = 15f;
    public float spread = 5f;
    private float spawnTimer = 0f;
    public int enemyCount = 0;
    private float healthMultiplier;

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
        foreach(var data in enemiesData){
            enemies.Add(data.prefab, data.weight);
        }
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
        
        List<GameObject> validEnemies = GetValidWavePool();
        GameObject enemyPrefab = GetRandomEnemy();
        Vector3 randomSpawnPosition = GetRandomPositionBehindCamera();
        if(enemyPrefab != null){
            GameObject spawnedEnemy = Instantiate(enemyPrefab, randomSpawnPosition, Quaternion.identity);
            spawnedEnemy.GetComponent<Enemy>().health *= healthMultiplier;
            enemyCount++;
        }
                
    }

    private Vector3 GetRandomPositionBehindCamera(){
        float height = 2f * camera.orthographicSize;
        float width = height * camera.aspect;

        float xThreshold = width / 2f;
        float yThreshold = height / 2f;

        float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        float spawnDistX = xThreshold + UnityEngine.Random.Range(minDistance, maxDistance);
        float spawnDistY = yThreshold + UnityEngine.Random.Range(minDistance, maxDistance);

        Vector3 spawnPos = new Vector3(
            camera.transform.position.x + (direction.x * spawnDistX),
            camera.transform.position.y + (direction.y * spawnDistY),
            0f 
        );

        return spawnPos;
    }

    // random enemy picker algorithm
    private GameObject GetRandomEnemy(){
        if(enemies.Count == 0){
          return null;   
        }
        float total = 0;
        foreach(float weight in enemies.Values){
            total += weight;
        }
        float randomVal = UnityEngine.Random.Range(0, total);
        float cursor = 0;
        foreach(var enemy in enemies){
            cursor += enemy.Value;
            if(randomVal <= cursor){
                return enemy.Key;
            }
        }
        return null;
    }

    public void IncreaseStats(float healthPoolIncrease, float spawnRateMultiplier){
        spawnRate *= spawnRateMultiplier;
        healthMultiplier *= healthPoolIncrease;
    }

    private List<GameObject> GetValidWavePool(){
        List<GameObject> pool = new();
        int currWave = GameManager.Instance.CurrWave;
        foreach(var enemy in enemies){
            Enemy e = enemy.Key.GetComponent<Enemy>();
            if(currWave == 1 && e.Type == EnemyType.Melee){
                pool.Add(enemy.Key);
            }
            else if(currWave == 2 && e.Type == EnemyType.Melee || e.Type == EnemyType.Ranged){
                pool.Add(enemy.Key);
            }
            else if(currWave >= 3){
                pool.Add(enemy.Key);
            }
        }
        return pool;
    }
}