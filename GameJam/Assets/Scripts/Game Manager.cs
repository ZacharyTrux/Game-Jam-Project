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
    private int defaultNextWave = 20;
    private float spawnRateMultiplier = 0.15f;
    private float healthPoolIncrease = 0.25f;
    private float maxEnemyMultiplier = 0.25f;
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
    }

    // Update is called once per frame
    void Update(){
        if(XPManager.Instance.TotalKills > defaultNextWave){
            CurrWave++;
            SetupNextWave();
        }
    }

    private void SetupNextWave(){
        EnemySpawnManager.Instance.IncreaseStats(healthPoolIncrease, spawnRateMultiplier);
        defaultNextWave += XPManager.Instance.TotalKills + (int)(defaultNextWave * maxEnemyMultiplier);
    }
}
