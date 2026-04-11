using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class XPManager : MonoBehaviour
{
    public static XPManager Instance { get; private set; }

    [Header("XP Settings")]
    public int xpPerKill = 10;
    public int xpToNextLevel = 50; // XP needed for first level up

    [Header("State")]
    public int CurrentXP { get; private set; } = 0;
    public int CurrentLevel { get; private set; } = 1;
    public int TotalKills { get; private set; } = 0;


    [Header("UI")]
    public Slider xpBar;
    public TextMeshProUGUI levelText;

    // Events for HUD and LevelUp screen to listen to
    public event System.Action<int, int> OnXPChanged;   // (currentXP, xpToNextLevel)
    public event System.Action<int> OnLevelUp;          // (newLevel)

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddKill()
    {
        TotalKills++;
        CurrentXP += xpPerKill;
        Debug.Log($"XP: {CurrentXP}/{xpToNextLevel} | Kills: {TotalKills} | Level: {CurrentLevel}");
        OnXPChanged?.Invoke(CurrentXP, xpToNextLevel);

        if (CurrentXP >= xpToNextLevel) LevelUp();

        UpdateUI();
    }

    // Took help from claude to implement the level up system - Sarun
    private void LevelUp()
    {
        CurrentXP -= xpToNextLevel;
        CurrentLevel++;

        // Each level requiring 10 percent more XP than the last level 
        // can be changed for us to cgeck for dev purpose - Sarun
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.1f);

        Debug.Log($"Level Up! Now level {CurrentLevel}");
        OnLevelUp?.Invoke(CurrentLevel);

        // Pause game and show upgrade choices
        Time.timeScale = 0f;
        LevelUpScreen.Instance?.Show(CurrentLevel);
    }

    public void ResetXP()
    {
        CurrentXP = 0;
        CurrentLevel = 1;
        xpToNextLevel = 50;
        TotalKills = 0;
    }

    void UpdateUI()
    {
        xpBar.value = CurrentXP / xpToNextLevel;
        levelText.text = "Level " + CurrentLevel.ToString();
    }

}