using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public Slider sensitivitySlider;

    public GameObject settingsMenu;

    void Start()
    {
        LoadSettingsFromManager();
        ApplySettings();
    }

    public void closeSettings()
    {
        settingsMenu.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        SettingsManager.Instance.settingsData.volume = volume;
    }

    public void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        SettingsManager.Instance.settingsData.graphicsQuality = qualityIndex;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        SettingsManager.Instance.settingsData.isFullscreen = isFullscreen;
    }

    public void SetSensitivity(float sensitivity)
    {
        SettingsManager.Instance.SetSensitivity(sensitivity);
        sensitivitySlider.value = sensitivity;
    }

    public void ApplySettings()
    {
        SetVolume(volumeSlider.value);
        SetFullscreen(fullscreenToggle.isOn);
        SetSensitivity(sensitivitySlider.value);
    }

    public void SaveSettings()
    {
        SettingsManager.Instance.SaveSettings();
    }

    private void LoadSettingsFromManager()
    {
        var settings = SettingsManager.Instance.settingsData;
        volumeSlider.value = settings.volume;
        fullscreenToggle.isOn = settings.isFullscreen;
        sensitivitySlider.value = settings.sensitvity;
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
    }
}

