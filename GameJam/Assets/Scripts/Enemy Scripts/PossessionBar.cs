using UnityEngine;
using UnityEngine.UI;

public class PossessionBar : MonoBehaviour
{
    [Header("UI")]
    public GameObject barObject;      // canvas and fill bar
    public Image fillImage;           // fill image stuff

    private float holdDuration = 2f; // need to hold to possess right now it is 2 seconds, can be changed in inspector
    private float currentHold = 0f;
    private bool isBeingTargeted = false;

    void Start()
    {
        if (barObject != null) barObject.SetActive(false);
    }

    void Update()
    {
        if (!isBeingTargeted)
        {
            // Drain bar when not targeted
            currentHold = Mathf.Max(0f, currentHold - Time.deltaTime * 2f);
            UpdateFill();
            if (currentHold <= 0f && barObject != null)
                barObject.SetActive(false);
            return;
        }

        // Fill bar while being targeted
        currentHold += Time.deltaTime;
        UpdateFill();

        if (barObject != null) barObject.SetActive(true);

        if (currentHold >= holdDuration)
        {
            TriggerPossession();
        }
    }

    private void UpdateFill()
    {
        if (fillImage != null)
            fillImage.fillAmount = currentHold / holdDuration;
    }

    private void TriggerPossession()
    {
        currentHold = 0f;
        isBeingTargeted = false;
        if (barObject != null) barObject.SetActive(false);

        Enemy enemy = GetComponent<Enemy>();
        if (enemy == null) return;
        if (enemy.Type == EnemyType.Boss || enemy.Type == EnemyType.Elite) return;
        if (MindControl.Instance == null || !MindControl.Instance.CanControl) return;

        MindControl.Instance.TryControl(enemy);
        SoundManager.Instance?.PlayPossess();
    }

    public void SetTargeted(bool targeted)
    {
        isBeingTargeted = targeted;
    }
}