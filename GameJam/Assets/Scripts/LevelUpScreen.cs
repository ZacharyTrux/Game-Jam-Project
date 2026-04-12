using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpScreen : MonoBehaviour
{
    public static LevelUpScreen Instance { get; private set; }

    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI levelText;
    public Button[] choiceButtons;
    public TextMeshProUGUI[] choiceLabels;

    // Your teammate hooks into this to scale enemy spawning
    public static event System.Action<int> OnLevelUpConfirmed;

    // Level-specific upgrade pools
    private string[][] level1Upgrades = new string[][]
    {
        new string[] { "Control +1 Enemy", "Faster Move Speed", "Longer Control Duration" },
        new string[] { "Control +1 Enemy", "XP Boost (1.5x)", "Pushback Force Up" },
        new string[] { "Faster Move Speed", "Longer Control Duration", "XP Boost (1.5x)" },
    };

    private string[][] level2Upgrades = new string[][]
    {
        new string[] { "Control +2 Enemies", "Faster Move Speed", "Instant Cooldown Reset" },
        new string[] { "Control +1 Enemy", "XP Boost (2x)", "Longer Control Duration" },
        new string[] { "Control +2 Enemies", "Pushback Force Up", "XP Boost (2x)" },
    };

    private string[][] level3Upgrades = new string[][]
    {
        new string[] { "Control +2 Enemies", "Instant Cooldown Reset", "XP Boost (2x)" },
        new string[] { "Control +3 Enemies", "Faster Move Speed", "Longer Control Duration" },
        new string[] { "Control +2 Enemies", "XP Boost (2x)", "Instant Cooldown Reset" },
    };

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(int newLevel)
    {
        panel.SetActive(true);
        levelText.text = $"Level {newLevel}!";

        // Pick upgrade pool based on level
        string[][] pool = newLevel == 1 ? level1Upgrades :
                          newLevel == 2 ? level2Upgrades :
                          level3Upgrades;

        string[] options = pool[Random.Range(0, pool.Length)];

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            string option = options[i];
            choiceLabels[i].text = option;
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(option, newLevel));
        }
    }


    private void OnChoiceSelected(string choice, int level)
    {
        switch (choice)
        {
            case "Control +1 Enemy":
                MindControl.Instance.IncreaseMaxControl(1);
                break;
            case "Control +2 Enemies":
                MindControl.Instance.IncreaseMaxControl(2);
                break;
            case "Control +3 Enemies":
                MindControl.Instance.IncreaseMaxControl(3);
                break;
            case "Faster Move Speed":
                Player.Instance.IncreaseMoveSpeed(1.5f);
                break;
            case "Longer Control Duration":
                MindControl.Instance.vulnerabilityDuration += 3f;
                break;
            case "XP Boost (1.5x)":
                XPManager.Instance.xpPerKill = Mathf.RoundToInt(XPManager.Instance.xpPerKill * 1.5f);
                break;
            case "XP Boost (2x)":
                XPManager.Instance.xpPerKill *= 2;
                break;
            case "Instant Cooldown Reset":
                // Resets vulnerability so player can possess again immediately
                MindControl.Instance.ResetVulnerability();
                break;
            case "Pushback Force Up":
                // Hook into pushback when ready
                break;
        }

        // Fire event so teammate can scale enemy spawning
        OnLevelUpConfirmed?.Invoke(level);

        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}