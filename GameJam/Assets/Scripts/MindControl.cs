using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MindControl : MonoBehaviour
{
    public static MindControl Instance { get; private set; }

    [Header("Control Settings")]
    public float vulnerabilityDuration = 5f;

    public bool IsVulnerable { get; private set; } = false;
    public bool CanControl => !IsVulnerable && controlledEnemies.Count < maxControlledEnemies;

    public List<Enemy> controlledEnemies = new();
    private float vulnerabilityTimer = 0f;

    // Events for HUD / player visuals to subscribe to
    public event System.Action<float> OnVulnerabilityTick; // fires each frame during cooldown (1 → 0)
    public event System.Action OnControlLost;
    private int baseControlledEnemies = 3;
    private int maxControlledEnemies = 3;


    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        if (!IsVulnerable) return;

        vulnerabilityTimer -= Time.deltaTime;
        OnVulnerabilityTick?.Invoke(vulnerabilityTimer / vulnerabilityDuration);

        if (vulnerabilityTimer <= 0f)
        {
            IsVulnerable = false;
            OnVulnerabilityTick?.Invoke(0f);
        }
    }

    public bool TryControl(Enemy enemy)
    {
        if (enemy == null) return false;
        if (!CanControl) return false;

        controlledEnemies.Add(enemy);
        enemy.MakePossesed();
        return true;
    }

    public void ReleaseEnemy(Enemy enemy)
    {
        controlledEnemies.Remove(enemy);

        if (controlledEnemies.Count == 0)
        {
            OnControlLost?.Invoke();
            TriggerVulnerability();
        }
    }

    private void TriggerVulnerability()
    {
        IsVulnerable = true;
        vulnerabilityTimer = vulnerabilityDuration;
        SoundManager.Instance?.PlayReleased();
    }

    public void IncreaseMaxControl(int num) => maxControlledEnemies += num;

    // for the level up feature 
    public void ResetVulnerability()
    {
        IsVulnerable = false;
        vulnerabilityTimer = 0f;
        OnVulnerabilityTick?.Invoke(0f);
    }

    public void ResetGame() => maxControlledEnemies = baseControlledEnemies;

}
