using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject[] menuObjects;
    [SerializeField] private Button[] defaultButtons;
    [SerializeField] private AudioClip selectSound;
    [SerializeField] private AudioClip pressSound;
    private AudioSource audioSource;
    private int currentMenu = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

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
        //defaultButtons[0].Select();
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

    public void OnButtonPressed()
    {
        audioSource.PlayOneShot(pressSound);
    }

    public void OnButtonSelected()
    {
        audioSource.PlayOneShot(selectSound);
    }
}
