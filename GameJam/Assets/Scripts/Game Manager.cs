using System;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour{
    public static GameManager Instance {get; private set;}
    public int currWave { get => currWave;
            set { currWave = value;
                waveText.text = "Wave: " + currWave;
                }
        }
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
        currWave = 1;
    }

    // Update is called once per frame
    void Update(){
        if(XPManager.Instance.TotalKills > defaultNextWave){
            currWave++;
            SetupNextWave();
        }
    }

    private void SetupNextWave(){
        EnemySpawnManager.Instance.IncreaseStats(healthPoolIncrease, spawnRateMultiplier);
        defaultNextWave += XPManager.Instance.TotalKills + (int)(defaultNextWave * maxEnemyMultiplier);
    }
}
