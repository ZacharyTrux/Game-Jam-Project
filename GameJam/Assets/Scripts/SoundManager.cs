using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Music")]
    public AudioClip backgroundMusic;
    public AudioClip bossMusicClip;
    public AudioClip winMusic;
    public AudioClip loseMusic;

    [Header("Player SFX")]
    public AudioClip possessSound;
    public AudioClip releasedSound;
    public AudioClip pushBackSound;

    [Header("Progression SFX")]
    public AudioClip levelUpSound;
    public AudioClip killSound;

    [Header("Power Up SFX")]
    public AudioClip powerUpControlSound;    
    public AudioClip powerUpSpeedSound;      
    public AudioClip powerUpDurationSound;   
    public AudioClip powerUpXPSound;         
    public AudioClip powerUpCooldownSound;   
    public AudioClip powerUpPushbackSound;   

    [Header("Enemy SFX")]
    public AudioClip meleeAttackSound;
    public AudioClip rangedAttackSound;
    public AudioClip tankAttackSound;
    public AudioClip bossAttackSound;
    public AudioClip enemyDeathSound;

    [Header("Settings")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    void Awake()
    {
    if (Instance != null) { Destroy(gameObject); return; }
    Instance = this;
    DontDestroyOnLoad(this);
    }

void Start()
    {
    sfxSource = gameObject.AddComponent<AudioSource>();
    musicSource = gameObject.AddComponent<AudioSource>();

    musicSource.loop = true;
    musicSource.volume = musicVolume;
    sfxSource.volume = sfxVolume;

    PlayMusic(backgroundMusic);
    }

    void OnEnable()
    {
        BossManager.OnBossSpawned += PlayBossMusic;
        LevelUpScreen.OnLevelUpConfirmed += PlayLevelUp;
    }

    void OnDisable()
    {
        BossManager.OnBossSpawned -= PlayBossMusic;
        LevelUpScreen.OnLevelUpConfirmed -= PlayLevelUp;
    }

    // --- Player ---
    public void PlayPossess()    => PlaySFX(possessSound);
    public void PlayReleased()   => PlaySFX(releasedSound);
    public void PlayPushBack()   => PlaySFX(pushBackSound);

    // --- Progression ---
    public void PlayKill()       => PlaySFX(killSound);
    public void PlayEnemyDeath() => PlaySFX(enemyDeathSound);
    private void PlayLevelUp(int level) => PlaySFX(levelUpSound);

    // --- Power Ups ---
    public void PlayPowerUp(string choice)
    {
        switch (choice)
        {
            case "Control +1 Enemy":
            case "Control +2 Enemies":
            case "Control +3 Enemies":
                PlaySFX(powerUpControlSound); break;
            case "Faster Move Speed":
                PlaySFX(powerUpSpeedSound); break;
            case "Longer Control Duration":
                PlaySFX(powerUpDurationSound); break;
            case "XP Boost (1.5x)":
            case "XP Boost (2x)":
                PlaySFX(powerUpXPSound); break;
            case "Instant Cooldown Reset":
                PlaySFX(powerUpCooldownSound); break;
            case "Pushback Force Up":
                PlaySFX(powerUpPushbackSound); break;
        }
    }

    // --- Enemy ---
    public void PlayMeleeAttack()  => PlaySFX(meleeAttackSound);
    public void PlayRangedAttack() => PlaySFX(rangedAttackSound);
    public void PlayTankAttack()   => PlaySFX(tankAttackSound);
    public void PlayBossAttack()   => PlaySFX(bossAttackSound);

    // --- Music ---
    private void PlayBossMusic()
    {
        if (bossMusicClip == null) return;
        musicSource.clip = bossMusicClip;
        musicSource.Play();
    }

    public void PlayWinMusic()
    {
        StopMusic();
        PlaySFX(winMusic);
    }

    public void PlayLoseMusic()
    {
        StopMusic();
        PlaySFX(loseMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic() => musicSource.Stop();

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = volume;
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }
}