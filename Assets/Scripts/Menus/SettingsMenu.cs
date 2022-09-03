using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown presetDropdown = null;
    [SerializeField] private TMP_Dropdown resolutionsDropdown = null;
    [SerializeField] private TMP_Dropdown displayDropdown = null;
    private Resolution[] resolutions;

    public void LoadSettings()
    {
        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        resolutionsDropdown.ClearOptions();

        List<string> resolutionStrings = new List<string>();
        int currentResolutionIndex = 0;
        int targetResolutionIndex = -1;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolutionString = $"{resolutions[i].width}x{resolutions[i].height}";
            resolutionStrings.Add(resolutionString);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }

            if (resolutions[i].width == 1280 && resolutions[i].height == 720)
            {
                targetResolutionIndex = i;
            }
        }
        resolutionsDropdown.AddOptions(resolutionStrings);

        if (PlayerPrefs.HasKey("Resolution"))
        {
            int index = PlayerPrefs.GetInt("Resolution");
            OnChangeResolution(index);
        }
        else if (targetResolutionIndex != 0)
        {
            OnChangeResolution(targetResolutionIndex);
        }
        else
        {
            OnChangeResolution(currentResolutionIndex);
        }

        int level = PlayerPrefs.HasKey("QualityLevel") ? PlayerPrefs.GetInt("QualityLevel") : 2;
        OnChangePreset(level);

        int displayMode = PlayerPrefs.HasKey("DisplayMode") ? PlayerPrefs.GetInt("DisplayMode") : 1;
        OnChangeDisplayMode(displayMode);
    }

    public void OnChangePreset(int preset)
    {
        QualitySettings.SetQualityLevel(preset);
        PlayerPrefs.SetInt("QualityLevel", preset);
        presetDropdown.value = preset;
    }

    public void OnChangeResolution(int resolution)
    {
        Screen.SetResolution(resolutions[resolution].width, resolutions[resolution].height, Screen.fullScreenMode);
        PlayerPrefs.SetInt("Resolution", resolution);
        resolutionsDropdown.value = resolution;
    }

    public void OnChangeDisplayMode(int mode)
    {
        if (mode == 0)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (mode == 1)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        displayDropdown.value = mode;
        PlayerPrefs.SetInt("DisplayMode", mode);
    }
}
