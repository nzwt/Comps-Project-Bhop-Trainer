using System;
using System.Collections;
using System.Collections.Generic;
using Fragsurf.Movement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Hud Elements
    [SerializeField]
    public GameObject HudElements;
    [SerializeField]
    public GameObject StartElements;
    [SerializeField]
    public GameObject StatScreen;
    [SerializeField]
    // Reset position values
    private float xReset = 0.0f;  // X-axis reset position
    [SerializeField]
    private float yReset = 1.0f;  // Y-axis reset position
    [SerializeField]
    private float zReset = 0.0f;  // Z-axis reset position
    [SerializeField]
    // Reference to the SurfCharacter script
    private SurfCharacter surfCharacter;
    [SerializeField]
    private PlayerAiming playerAiming;
    public ScoreManager scoreManager;
    
    public bool hasJumped = false;
    public int attemptNumber = 0;
    public bool firstFrame = true;
    public bool lastScoreLoaded = false;
    public bool startTriggered = false;
    //stats
    public JumpAttempt lastJumpAttempt = null;
    public JumpAttempt currentJumpAttempt;
    public MouseAngleTracker mouseAngleTracker;
    public SpeedTracker speedTracker;
    //scene bool
    //not sure if this is how I want to do this
    public bool allowPlayerMovement = true;


    void ResetPlayer()
    {
        // Reset the player's position and rotation
        surfCharacter.transform.position = new Vector3(xReset, yReset, zReset);
        surfCharacter.transform.rotation = Quaternion.LookRotation(Vector3.forward);
        //disable input
        surfCharacter.moveData.verticalAxis = 0;
        surfCharacter.moveData.horizontalAxis = 0;
        surfCharacter.movementEnabled = false;
        mouseAngleTracker.isAttemptActive = false;
        speedTracker.isAttemptActive = false;
        DisableMouseLook();
    }

    private void OnEnable()
    {
        currentJumpAttempt = new JumpAttempt(attemptNumber, 0, 0, 0, 0, 0, 0, 0, 0, date: System.DateTime.Now);
        DisableHudElements();
        DisableStatScreen();
        EnableStartElements();
        ResetPlayer();
    }

    private void OnDisable()
    {
    }

    //TODO get rid of these functions, not needed anymore, just toggle on and off individually
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
    
    public void DisableMouseLook()
    {
        playerAiming.resetRotation();
        playerAiming.canAim = false;
    }

    public void EnableMouseLook()
    {
        playerAiming.canAim = true;
    }

    public void resetScene()
    {
        ResetPlayer();
        DisableHudElements();
        EnableStatScreen();
        EnableStartElements();
        startTriggered = false;
    }

    public void startAttempt()
    {
        EnableHudElements();
        DisableStatScreen();
        DisableStartElements();
        surfCharacter.movementEnabled = true;
        EnableMouseLook();
        if(!allowPlayerMovement)
        {
            surfCharacter.controller.moveForward = true;
            //has to jump
        }
        mouseAngleTracker.isAttemptActive = true;
        speedTracker.isAttemptActive = true;
    }
    IEnumerator handleJump()
    {
        for(int i = 0; i < 10; i++)
        {
            yield return null;
        }
        hasJumped = true;
    }
    public void endAttempt()
    {
        //stop the movement
        if(!allowPlayerMovement)
        {
            surfCharacter.controller.moveForward = false;
            surfCharacter.controller.moveRight = false;
            surfCharacter.controller.attemptWishJump = false;
        }
        // Save the score, values are placeholders for now
        attemptNumber++;
        // Update the lastJumpAttempt to the currentJumpAttempt
        // Reset the currentJumpAttempt
        float score = speedTracker.CalculateAttemptSpeed() + (20 - Math.Abs(45 - mouseAngleTracker.CalculateAttemptAngleChange())); //+ mouseAngleTracker.CalculateAverageAttemptAngleSmoothness();//(10 - Math.Abs(mouseAngleTracker.CalculateAverageAttemptAngleSmoothness()));
        currentJumpAttempt = new JumpAttempt(attemptNumber, 0, 0, 0, 0, speedTracker.CalculateAttemptSpeed(), score, mouseAngleTracker.CalculateAttemptAngleChange(), mouseAngleTracker.CalculateAverageAttemptAngleSmoothness(), date: System.DateTime.Now);
        scoreManager.SaveScore(currentJumpAttempt);
        StatScreen.GetComponent<StatScreen>().updateStats();
        hasJumped = false;
        mouseAngleTracker.isAttemptActive = false;
        speedTracker.isAttemptActive = false;
        lastJumpAttempt = currentJumpAttempt;
        
        
    }

    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {   
        //skip first frame, otherwise the player will register an attempt
        bool grounded = surfCharacter.moveData.groundedTemp;
        if(firstFrame)
        {
            grounded = true;
            firstFrame = false;
        }
        //load the scores
        if(scoreManager.isLoaded == true && lastScoreLoaded == false && scoreManager.GetLastJumpAttempt() != null)
        {

            //load the most recent score
            lastJumpAttempt = scoreManager.GetLastJumpAttempt();
            attemptNumber = lastJumpAttempt.attemptNumber + 1;
            lastScoreLoaded = true; 
        }
        //Debug.Log(surfCharacter.transform.position.z);
        if(surfCharacter.transform.position.z <= 0.05 && surfCharacter.transform.position.z >= -0.05 && !startTriggered && !allowPlayerMovement)
        {
            Debug.Log("Reset");
            surfCharacter.controller.moveForward = false;
            surfCharacter.controller.attemptWishJump = true;
            surfCharacter.controller.moveRight = true;
            StartCoroutine(handleJump());
            startTriggered = true;
        }
        //Debug.Log(grounded);
            
        //TODO - need to update this to have checks for starting level
        if (Input.GetMouseButtonDown(0))
        {
            startAttempt();
        }
        if( hasJumped == false && grounded == false)
        {
            print("Jumped");
            hasJumped = true;
        }
        // if( hasJumped == true && grounded == false)
        // {
        //     surfCharacter.controller.moveForward = false;
        // }
        if (hasJumped == true && grounded == true)
        {
            endAttempt();
            resetScene();
        }
    }
}
