using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsMenu;
    public SettingsMenu settingsMenuScript;
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        settingsMenu.SetActive(false);
    }
    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
    }
    public void LoadScene()
    {
        SceneManager.LoadScene("Airstrafe");
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && settingsMenu.active == false)
        {
            settingsMenu.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && settingsMenu.active == true)
        {
            settingsMenuScript.ApplySettings();
            settingsMenuScript.SaveSettings();
            settingsMenu.SetActive(false);
        }
    }
}
