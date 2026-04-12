using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // scene transfer
    public GameObject followCamera;
    public GameObject canvas;
    public GameObject eventSys;
    public GameObject bossPrefab;


    public int CurrWave
    {
        get => _currWave;
        set
        {
            _currWave = value;
            if (waveText != null)
                waveText.text = "Wave: " + _currWave;
        }
    }

    private int _currWave;

    private int defaultNextWave = 3;
    private float spawnRateMultiplier = 1.15f;
    private float healthPoolIncrease = 1.25f;
    private float maxEnemyMultiplier = 1f;
    public TextMeshProUGUI waveText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(followCamera);
        DontDestroyOnLoad(canvas);
        DontDestroyOnLoad(eventSys);
    }

    void Start()
    {
        Debug.Log(waveText.text);
        CurrWave = 1;
        Time.timeScale = 1f;
        Player.Instance.EnableInput();
    }

    void Update()
    {
        if (XPManager.Instance != null && XPManager.Instance.TotalKills > defaultNextWave)
        {
            CurrWave++;
            if (CurrWave % 5 == 0) LevelUpScreen.Instance.ScaleToWave();

            if (CurrWave == 5 && SceneManager.GetActiveScene().name != "EndlessMode")
            {
                SceneManager.LoadScene("MindPalace");
                Player.Instance.DisableInput();
                Player.Instance.transform.position = Vector2.zero;
                Player.Instance.EnableInput();
            }
            if (CurrWave == 10 && SceneManager.GetActiveScene().name == "MindPalace")
            {
                Instantiate(bossPrefab);
            }
            
            SetupNextWave();
        }
    }

    private void SetupNextWave()
    {
        EnemySpawnManager.Instance.IncreaseStats(healthPoolIncrease, spawnRateMultiplier);
        defaultNextWave += XPManager.Instance.TotalKills + (int)(defaultNextWave * maxEnemyMultiplier);
    }
}