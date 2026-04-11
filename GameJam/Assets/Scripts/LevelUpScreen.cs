using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpScreen : MonoBehaviour
{
    public static LevelUpScreen Instance { get; private set; }

    [Header("UI References")]
    public GameObject panel;                    // The popup panel
    public TextMeshProUGUI levelText;           // "Level 2!" text
    public Button[] choiceButtons;             // 3 buttons for upgrade choices
    public TextMeshProUGUI[] choiceLabels;     // Labels on each button

    // Define your upgrade options here
    private string[][] upgradeOptions = new string[][]
    {
        new string[] { "Control +1 Enemy", "Faster Move Speed", "Longer Control Duration" },
        new string[] { "Control +1 Enemy", "XP Boost (2x)", "Pushback Range Up" },
        new string[] { "Control +2 Enemies", "Instant Cooldown Reset", "Enemy Spawn Slows" },
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

        // Pick a random set of 3 upgrades
        int setIndex = Random.Range(0, upgradeOptions.Length);
        string[] options = upgradeOptions[setIndex];

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int captured = i;
            string option = options[i];
            choiceLabels[i].text = option;
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(option));
        }
    }

    private void OnChoiceSelected(string choice)
    {
        // Apply the chosen upgrade
        switch (choice)
        {
            case "Control +1 Enemy":
                MindControl.Instance.increaseMaxControl(1);
                break;
            case "Control +2 Enemies":
                MindControl.Instance.increaseMaxControl(2);
                break;
            case "Faster Move Speed":
                Player.Instance.IncreaseMoveSpeed(1.5f); break;
            case "Longer Control Duration":
                MindControl.Instance.vulnerabilityDuration += 3f;
                break;
            case "XP Boost (2x)":
                XPManager.Instance.xpPerKill *= 2;
                break;
            case "Pushback Range Up":
                // Hook into pushback when ready
                break;
            case "Instant Cooldown Reset":
                // Reset vulnerability immediately
                break;
        }

        panel.SetActive(false);
        Time.timeScale = 1f; // Resume game
    }
}


