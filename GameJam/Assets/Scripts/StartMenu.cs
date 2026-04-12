using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] menus;

    [Header("Sound Controls")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI muteButtonText;

    private bool isMuted = false;

    void Start()
    {
        if (SoundManager.Instance != null)
        {
            if (musicVolumeSlider != null)
                musicVolumeSlider.value = SoundManager.Instance.musicVolume;
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.value = SoundManager.Instance.sfxVolume;
        }
    }

    public void ToggleMenu(int menuIndex) => menus[menuIndex].SetActive(!menus[menuIndex].activeSelf);

    public void CloseMenus()
    {
        foreach (GameObject menu in menus)
            menu.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Test1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnMusicVolumeChanged(float value)
    {
        SoundManager.Instance?.SetMusicVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        SoundManager.Instance?.SetSFXVolume(value);
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;

        if (isMuted)
        {
            SoundManager.Instance?.SetMusicVolume(0f);
            SoundManager.Instance?.SetSFXVolume(0f);
            if (muteButtonText != null) muteButtonText.text = "Unmute";
        }
        else
        {
            SoundManager.Instance?.SetMusicVolume(musicVolumeSlider.value);
            SoundManager.Instance?.SetSFXVolume(sfxVolumeSlider.value);
            if (muteButtonText != null) muteButtonText.text = "Mute";
        }
    }
}