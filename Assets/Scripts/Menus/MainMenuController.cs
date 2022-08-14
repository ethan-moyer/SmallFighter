using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject[] menuObjects;
    [SerializeField] private Button[] defaultButtons;
    private int currentMenu = 0;

    private void Awake()
    {
        foreach (GameObject menu in menuObjects)
        {
            SettingsMenu settingsMenu = menu.GetComponent<SettingsMenu>();
            if (settingsMenu != null)
            {
                settingsMenu.LoadSettings();
            }
            menu.SetActive(false);
        }
        menuObjects[0].SetActive(true);
        defaultButtons[0].Select();
    }

    public void OnPlayPressed()
    {
        SceneManager.LoadScene("Scenes/DevicesScreen", LoadSceneMode.Single);
    }

    public void OnMenuButtonPressed(int nextMenu)
    {
        menuObjects[currentMenu].SetActive(false);
        menuObjects[nextMenu].SetActive(true);
        defaultButtons[nextMenu].Select();
        currentMenu = nextMenu;
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }
}
