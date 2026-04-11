using System.Collections.Generic;
using UnityEngine;

public class MindControl : MonoBehaviour
{
    public static MindControl Instance { get; private set; }

    [Header("Control Settings")]
    public float controlDuration = 10f;
    public float vulnerabilityDuration = 10f;

    public bool IsVulnerable { get; private set; } = false;
    public bool CanControl => !IsVulnerable && controlledEnemies.Count == 0;

    private List<EnemyScript> controlledEnemies = new();
    private float vulnerabilityTimer = 0f;

    // Events for HUD / player visuals to subscribe to
    public event System.Action<float> OnVulnerabilityTick; // fires each frame during cooldown (1 → 0)
    public event System.Action OnControlLost;

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

    public bool TryControl(EnemyScript enemy)
    {
        if (!CanControl) return false;

        controlledEnemies.Add(enemy);
        // enemy.BecomePossessed(controlDuration);  //Needs to be implemented in EnemyScript
        return true;
    }

    public void OnEnemyFreed(EnemyScript enemy)
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
    }
}