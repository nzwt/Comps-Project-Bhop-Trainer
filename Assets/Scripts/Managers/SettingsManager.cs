using UnityEngine;
using System.IO;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    public SettingsData settingsData;

    private string settingsFilePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            settingsFilePath = Path.Combine(Application.persistentDataPath, "settings.json");
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadSettings()
    {
        if (File.Exists(settingsFilePath))
        {
            string json = File.ReadAllText(settingsFilePath);
            settingsData = JsonUtility.FromJson<SettingsData>(json);
        }
        else
        {
            settingsData = new SettingsData
            {
                volume = 0.5f,
                sensitvity = 1.0f,
                graphicsQuality = 2,
                isFullscreen = true
            };
        }
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(settingsData, true);
        File.WriteAllText(settingsFilePath, json);
    }

    public float GetSensitivity() => settingsData.sensitvity;
    public void SetSensitivity(float sensitivity) => settingsData.sensitvity = sensitivity;
}

