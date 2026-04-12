using System;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour{
    public static GameManager Instance {get; private set;}
    public int CurrWave { get => _currWave;
            set { _currWave = value;
                if(waveText != null){
                    waveText.text = "Wave: " + _currWave;
                }
            }
        }

    private int _currWave;
    private int defaultNextWave = 10;
    private float spawnRateMultiplier = 1.15f;
    private float healthPoolIncrease = 1.25f;
    private float maxEnemyMultiplier = 1.25f;
    public TextMeshProUGUI waveText;
    
    void Awake(){
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start(){
        Debug.Log(waveText.text);
        CurrWave = 1;
        Time.timeScale = 1f;
        Player.Instance.EnableInput();
    }

    // Update is called once per frame
    void Update(){
        if(XPManager.Instance != null && XPManager.Instance.TotalKills > defaultNextWave){
            CurrWave++;
            if (CurrWave % 5 == 0) LevelUpScreen.Instance.ScaleToWave();
            SetupNextWave();
        }
    }

    private void SetupNextWave(){
        EnemySpawnManager.Instance.IncreaseStats(healthPoolIncrease, spawnRateMultiplier);
        defaultNextWave += XPManager.Instance.TotalKills + (int)(defaultNextWave * maxEnemyMultiplier);
    }
}
