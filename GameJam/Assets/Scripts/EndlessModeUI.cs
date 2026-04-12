using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndlessModeUI : MonoBehaviour
{
    public static EndlessModeUI Instance { get; private set; }

    public GameObject gameOverPanel;

    public Slider healthBar;
    private void Awake()
    {
        Instance = this;
    }

    public void GameOver()
    {
        // player death animation
        Time.timeScale = 0f;
        Player.Instance.DisableInput();
        gameOverPanel.SetActive(true);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void UpdateHealth()
    {
        healthBar.value = (float)Player.Instance.health / Player.Instance.maxHealth;
    }

}
