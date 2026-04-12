using UnityEngine;

public class CheatCodes : MonoBehaviour
{
    [Header("Cheat Settings")]
    public KeyCode killAllKey = KeyCode.K;  // Hold Ctrl + K to kill all enemies

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(killAllKey))
        {KillAllEnemies();}
    }

    private void KillAllEnemies()
    {
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in allEnemies)
        {if (!enemy.isPossessed)
                enemy.TakeDamage(99999f);}
        Debug.Log($"[CHEAT] Killed {allEnemies.Length} enemies");
    }
}