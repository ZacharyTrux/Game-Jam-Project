using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Clips")]
    public AudioClip possessSound;
    public AudioClip releasedSound;
    public AudioClip pushBackSound;
    public AudioClip levelUpSound;
    public AudioClip enemyDeathSound;
    public AudioClip bossMusicClip;

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

        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
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

    // --- Public methods to call from anywhere ---

    public void PlayPossess()    => PlaySFX(possessSound);
    public void PlayReleased()   => PlaySFX(releasedSound);
    public void PlayPushBack()   => PlaySFX(pushBackSound);
    public void PlayEnemyDeath() => PlaySFX(enemyDeathSound);

    private void PlayLevelUp(int level) => PlaySFX(levelUpSound);

    private void PlayBossMusic()
    {
        if (bossMusicClip == null) return;
        musicSource.clip = bossMusicClip;
        musicSource.Play();
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