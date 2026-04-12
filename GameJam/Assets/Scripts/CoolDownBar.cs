using UnityEngine;
using UnityEngine.UI;

public class CooldownBarUI : MonoBehaviour
{
    public Slider cooldownSlider;

    void Start()
    {
        if (MindControl.Instance != null)
            MindControl.Instance.OnVulnerabilityTick += UpdateBar;
        if (cooldownSlider != null)
            cooldownSlider.value = 1f;
    }

    void OnDestroy()
    {
        if (MindControl.Instance != null)
            MindControl.Instance.OnVulnerabilityTick -= UpdateBar;
    }

    private void UpdateBar(float t)
    {
        if (cooldownSlider != null)
            cooldownSlider.value = 1f - t;
    }
}