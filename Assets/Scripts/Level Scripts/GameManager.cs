using System.Collections;
using System.Collections.Generic;
using Fragsurf.Movement;
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
    [SerializeField]
    private SurfCharacter surfCharacter;
    [SerializeField]
    private PlayerAiming playerAiming;
    
    public bool hasJumped = false;

    void ResetPlayer()
    {
        // Reset the player's position and rotation
        surfCharacter.transform.position = new Vector3(xReset, yReset, zReset);
        surfCharacter.transform.rotation = Quaternion.LookRotation(Vector3.forward);
        //disable input
        surfCharacter.moveData.verticalAxis = 0;
        surfCharacter.moveData.horizontalAxis = 0;
        surfCharacter.movementEnabled = false;
        //DisableMouseLook();
    }

    private void OnEnable()
    {
        DisableHudElements();
        EnableStartElements();
        ResetPlayer();
    }

    private void OnDisable()
    {
    }

    public void EnableHudElements()
    {
        for (int i = 0; i < HudElements.Length; i++)
        {
            HudElements[i].SetActive(true);
        }
    }

    public void DisableHudElements()
    {
        for (int i = 0; i < HudElements.Length; i++)
        {
            HudElements[i].SetActive(false);
        }
    }
    
    public void EnableStartElements()
    {
        for (int i = 0; i < StartElements.Length; i++)
        {
            StartElements[i].SetActive(true);
        }
    }

    public void DisableStartElements()
    {
        for (int i = 0; i < StartElements.Length; i++)
        {
            StartElements[i].SetActive(false);
        }
    }
    
    public void DisableMouseLook()
    {
        //playerAiming.enabled = false;
    }

    public void EnableMouseLook()
    {
        //playerAiming.enabled = true;
    }

    public void resetScene()
    {
        ResetPlayer();
        DisableHudElements();
        EnableStartElements();
    }

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        bool grounded = surfCharacter.moveData.groundedTemp;
        print(grounded);
        //TODO - need to update this to have checks
        if (Input.GetMouseButtonDown(0))
        {
            EnableHudElements();
            DisableStartElements();
            surfCharacter.movementEnabled = true;
            EnableMouseLook();
        }
        if( hasJumped == false && grounded == false)
        {
            print("Jumped");
            hasJumped = true;
        }
        if (hasJumped == true && grounded == true)
        {
            hasJumped = false;
            resetScene();
        }
    }
}
