using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    // Set in inspector
    [SerializeField] private GameObject[] menus;

    public void ToggleMenu(int menuIndex) => menus[menuIndex].SetActive(!menus[menuIndex].activeSelf);
    

    public void CloseMenus()
    {
        foreach (GameObject menu in menus)
        {
            menu.SetActive(false);
        }
    }

    // Start Menu funcs
    public void StartGame()
    {
        SceneManager.LoadScene("Test1"); // Replace this with level 1
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
