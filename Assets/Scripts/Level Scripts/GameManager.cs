using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public GameObject[] HudElements = new GameObject[3];
    [SerializeField]
    public GameObject[] StartElements = new GameObject[3];
    [SerializeField]
    private float xReset = 0.0f;  // X-axis reset position
    [SerializeField]
    private float yReset = 1.0f;  // Y-axis reset position
    [SerializeField]
    private float zReset = 0.0f;  // Z-axis reset position

    void ResetPlayer()
    {
        // Reset the player's position and rotation
        transform.position = new Vector3(xReset, yReset, zReset);
        transform.rotation = Quaternion.identity;
    }

    private void OnEnable()
    {
        // Subscribe to the event
        for ( int i = 0; i < HudElements.Length; i++)
        {
            HudElements[i].SetActive(false);
        }
        for (int i = 0; i < StartElements.Length; i++)
        {
            StartElements[i].SetActive(true);
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from the event
    
    }

    public void EnableHudElements()
    {
        for (int i = 0; i < HudElements.Length; i++)
        {
            HudElements[i].SetActive(true);
        }
    }

    public void DisableStartElements()
    {
        for (int i = 0; i < StartElements.Length; i++)
        {
            StartElements[i].SetActive(false);
        }
    }

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        //TODO - need to update this to have checks
        if (Input.GetMouseButtonDown(0))
        {
            EnableHudElements();
            DisableStartElements();
        }
    }
}
