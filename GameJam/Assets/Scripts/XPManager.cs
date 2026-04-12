using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPManager : MonoBehaviour
{
    public static XPManager Instance { get; private set; }

    [Header("XP Settings")]
    public int xpPerKill = 10;
    public int bonusXP = 0;
    public int xpToNextLevel = 50;

    [Header("State")]
    public int CurrentXP { get; private set; } = 0;
    public int CurrentLevel { get; private set; } = 1;
    public int TotalKills { get; private set; } = 0;

    [Header("UI")]
    public Slider xpBar;
    public TextMeshProUGUI levelText;

    public event System.Action<int, int> OnXPChanged;
    public event System.Action<int> OnLevelUp;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddKill()
    {
        TotalKills++;
        CurrentXP += xpPerKill + bonusXP;
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
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.1f);

        Debug.Log($"Level Up! Now level {CurrentLevel}");
        OnLevelUp?.Invoke(CurrentLevel);

        Time.timeScale = 0f;
        LevelUpScreen.Instance?.Show(CurrentLevel);
    }

    public void ResetXP()
    {
        CurrentXP = 0;
        CurrentLevel = 1;
        xpToNextLevel = 50;
        TotalKills = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (xpBar != null)
            xpBar.value = (float)CurrentXP / xpToNextLevel;
        if (levelText != null)
            levelText.text = "Level " + CurrentLevel.ToString();
    }
}