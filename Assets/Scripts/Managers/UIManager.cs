using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //Hud Elements
    public GameObject HudElements;
    public GameObject StartElements;
    public GameObject StatScreen;
    public void EnableHudElements()
    {
        HudElements.SetActive(true);
    }

    public void DisableHudElements()
    {
        HudElements.SetActive(false);
    }
    
    public void EnableStartElements()
    { 
        StartElements.SetActive(true);
    }

    public void DisableStartElements()
    {
        StartElements.SetActive(false);
    }
    
    public void EnableStatScreen()
    {
        StatScreen.SetActive(true);
    }
    public void DisableStatScreen()
    {
        StatScreen.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
