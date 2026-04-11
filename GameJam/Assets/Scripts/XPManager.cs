using UnityEngine;
using UnityEngine.Events;

public class XPManager : MonoBehaviour
{
    public static XPManager Instance { get; private set; }

    [Header("XP Settings")]
    public int xpPerKill = 10;
    public int xpToNextLevel = 50; // XP needed for first level up

    [Header("State")]
    public int currentXP { get; private set; } = 0;
    public int currentLevel { get; private set; } = 1;
    public int totalKills { get; private set; } = 0;

    // Events for HUD and LevelUp screen to listen to
    public event System.Action<int, int> OnXPChanged;   // (currentXP, xpToNextLevel)
    public event System.Action<int> OnLevelUp;          // (newLevel)

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AddKill()
{
    totalKills++;
    currentXP += xpPerKill;
    Debug.Log($"XP: {currentXP}/{xpToNextLevel} | Kills: {totalKills} | Level: {currentLevel}");
    OnXPChanged?.Invoke(currentXP, xpToNextLevel);

    if (currentXP >= xpToNextLevel)
        LevelUp();
}

    private void LevelUp()
    {
        currentXP -= xpToNextLevel;
        currentLevel++;

        // Each level requiring 10 percent more XP than the last level 
        // can be changed for us to cgeck for dev purpose - Sarun
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.1f);

        Debug.Log($"Level Up! Now level {currentLevel}");
        OnLevelUp?.Invoke(currentLevel);

        // Pause game and show upgrade choices
        Time.timeScale = 0f;
        LevelUpScreen.Instance?.Show(currentLevel);
    }

    public void ResetXP()
    {
        currentXP = 0;
        currentLevel = 1;
        xpToNextLevel = 50;
        totalKills = 0;
    }
    
}