using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SettingsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public Dropdown graphicsDropdown;
    public Toggle fullscreenToggle;
    public Slider sensitivitySlider;

    private string settingsFilePath;
    public GameObject settingsMenu;

    void Start()
    {
        settingsFilePath = Path.Combine(Application.persistentDataPath, "settings.json");
        LoadSettings();

        // Apply loaded settings
        ApplySettings();
    }

    public void closeSettings()
    {
        settingsMenu.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetSensitivity(float sensitivity)
    {
        sensitivitySlider.value = sensitivity;
    }

    public void ApplySettings()
    {
        // Apply settings when the menu is loaded or changes are made
        SetVolume(volumeSlider.value);
        //SetGraphicsQuality(graphicsDropdown.value);
        SetFullscreen(fullscreenToggle.isOn);
        SetSensitivity(sensitivitySlider.value);
    }

    public void SaveSettings()
    {
        SettingsData settings = new SettingsData
        {
            volume = volumeSlider.value,
            //graphicsQuality = graphicsDropdown.value,
            isFullscreen = fullscreenToggle.isOn,
            sensitvity = sensitivitySlider.value
        };

        string json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(settingsFilePath, json);
    }

    public void LoadSettings()
    {
        if (File.Exists(settingsFilePath))
        {
            string json = File.ReadAllText(settingsFilePath);
            SettingsData settings = JsonUtility.FromJson<SettingsData>(json);

            volumeSlider.value = settings.volume;
            //graphicsDropdown.value = settings.graphicsQuality;
            fullscreenToggle.isOn = settings.isFullscreen;
            sensitivitySlider.value = settings.sensitvity;
        }
    }

    // You can call SaveSettings on a button click or when the application quits
    private void OnApplicationQuit()
    {
        SaveSettings();
    }
}
