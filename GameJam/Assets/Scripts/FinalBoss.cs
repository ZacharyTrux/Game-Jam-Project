using UnityEngine;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance { get; private set; }

    [Header("Boss Settings")]
    public GameObject bossPrefab;           
    public Transform bossSpawnPoint;        // Where the boss spawns
    public int wavesBeforeBoss = 3;         // 3 levels of gameplay and then the boss appears at level 3, after 3 waves

    private int currentWave = 0;
    private bool bossSpawned = false;
    private bool level3Reached = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnEnable()
    {
        LevelUpScreen.OnLevelUpConfirmed += OnLevelUp;
    }

    void OnDisable()
    {
        LevelUpScreen.OnLevelUpConfirmed -= OnLevelUp;
    }

    private void OnLevelUp(int level)
    {
        if (level >= 3)
        {
            level3Reached = true;
            Debug.Log("Level 3 reached — boss waves starting!");
        }
    }

    // Your teammate calls this from their wave spawner after each wave completes
    public void OnWaveCompleted()
    {
        if (!level3Reached || bossSpawned) return;

        currentWave++;
        Debug.Log($"Wave {currentWave}/{wavesBeforeBoss} completed at level 3");

        if (currentWave >= wavesBeforeBoss)
            SpawnBoss();
    }

    private void SpawnBoss()
    {
        if (bossSpawned) return;
        if (bossPrefab == null)
        {
            Debug.LogWarning("Boss prefab not assigned in BossManager!");
            return;
        }

        bossSpawned = true;
        Vector3 spawnPos = bossSpawnPoint != null ? bossSpawnPoint.position : Vector3.zero;
        GameObject boss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        Debug.Log("Mind Goblin has appeared!");

        // Fire event so UI/music can react
        OnBossSpawned?.Invoke();
    }

    public static event System.Action OnBossSpawned;

    public void ResetBoss()
    {
        currentWave = 0;
        bossSpawned = false;
        level3Reached = false;
    }
}