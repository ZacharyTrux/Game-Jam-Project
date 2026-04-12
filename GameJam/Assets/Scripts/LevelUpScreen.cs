using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;


public enum UpgradeTypes
{
    CONTROL,
    HEALTH,
    DMG,
    SPEED,
    XP
}

public class Upgrade
{
    public UpgradeTypes type;
    public string desc;
    public int weight;

    public Upgrade(UpgradeTypes _type, string _desc, int _weight)
    {
        type = _type;
        desc = _desc;
        weight = _weight;
    }
}


public class LevelUpScreen : MonoBehaviour
{
    public static LevelUpScreen Instance { get; private set; }

    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI levelText;
    public Button[] choiceButtons;
    public TextMeshProUGUI[] choiceLabels;

// Took help from Claude and Youtube for Level Up Screen.
    public static event System.Action<int> OnLevelUpConfirmed;

    // Upgrade Setup (Scaling Upgrades)
    // CONTROL
    public int upgradeScaleControl = 1;
    // Health / Regen
    public float upgradeScaleHealth = 10f;
    public float upgradeScaleHealthRegen = 1f;

    // Non-Scaling Upgrades
    public float pushAttackDmgUpgrade = 1f;
    public float speedUpgrade = 0.2f;

    // Upgrade Descriptions
    public string upgradeDescControl;
    public string upgradeDescHealth;
    public string upgradeDescDMG;
    public string upgradeDescSpeed;
    public string upgradeDescXP;

    public List<Upgrade> availibleUpgrades = new();
    public Upgrade speed;
    public Upgrade health;
    public Upgrade xp;
    public Upgrade dmg;
    public Upgrade control;


    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        panel.SetActive(false);
    }

    private void Start()
    {
        speed = new(UpgradeTypes.SPEED, $"Speed Bonus of +{speedUpgrade}", 5);
        health = new(UpgradeTypes.HEALTH, $"Gain +{upgradeScaleHealth} max health and {upgradeScaleHealthRegen} bonus health regen", 20);
        dmg = new(UpgradeTypes.DMG, $"Increase Physic Wave Damage by +1", 10);
        xp = new(UpgradeTypes.XP, $"Gain +10% base bonus XP from enemies", 5);
        control = new(UpgradeTypes.CONTROL, $"Control +{upgradeScaleControl} enemies(s)", 60);
        availibleUpgrades.Add(speed);
        availibleUpgrades.Add(health);
        availibleUpgrades.Add(dmg);
        availibleUpgrades.Add(xp);
        availibleUpgrades.Add(control);
    }


    public List<Upgrade> GenerateUpgradePool()
    {
        List<Upgrade> pool = new(availibleUpgrades);
        List<Upgrade> choices = new();
        while (choices.Count < 3 && pool.Count > 0)
        {
            Upgrade choice = WeightCalc(pool);
            choices.Add(choice);
            pool.Remove(choice);
        }
        return choices;
    }

    public Upgrade WeightCalc(List<Upgrade> list)
    {
        int totalWeight = 0;
        foreach (Upgrade upgrade in list) totalWeight += upgrade.weight;
        int r = Random.Range(0, totalWeight);
        int cursor = 0;
        foreach (var u in list)
        {
            cursor += u.weight;
            if (r < cursor) return u;
        }
        return list[0];
    }

    public void Show(int newLevel)
    {
        panel.SetActive(true);
        levelText.text = $"Level {newLevel}!";

        var choices = GenerateUpgradePool();
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            Upgrade option = choices[i];
            choiceLabels[i].text = option.desc;
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(option.type));
        }
    }


    private void OnChoiceSelected(UpgradeTypes type)
    {
        switch (type)
        {
            case UpgradeTypes.HEALTH:
                Player.Instance.IncreaseHealth(upgradeScaleHealth, upgradeScaleHealthRegen);
                break;
            case UpgradeTypes.DMG:
                Player.Instance.damage += 1;
                break;
            case UpgradeTypes.SPEED:
                Player.Instance.IncreaseMoveSpeed(speedUpgrade);
                break;
            case UpgradeTypes.CONTROL:
                MindControl.Instance.IncreaseMaxControl(upgradeScaleControl);
                break;
            case UpgradeTypes.XP:
                XPManager.Instance.bonusXP += 1;
                break;
        }

        panel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ScaleToWave()
    {
        upgradeScaleControl += 1;
        upgradeScaleHealth += 5;
        upgradeScaleHealthRegen += 1;
    }

}