using System.Collections;
using System.Collections.Generic;
using Fragsurf.Movement;
using UnityEngine;

public class AirstrafeSceneManager : MonoBehaviour
{
    // refrences to other scripts
    public SurfCharacter surfCharacter;
    public UIManager uiManager;
    public PlayerManager playerManager;
    public ScoreManager scoreManager;

    // game objects
    public JumpAttempt currentJumpAttempt;
    public JumpAttempt lastJumpAttempt;
    public int attemptNumber = 0;
    public MouseAngleTracker mouseAngleTracker;
    public SpeedTracker speedTracker;
    
    // managment bools
    public bool hasJumped = false;
    public bool firstFrame = true;
    public bool lastScoreLoaded = false;
    public bool startTriggered = false;
    public bool allowPlayerMovement = true;

    // Reset position values
    public float xReset = 0.0f;  // X-axis reset position
    public float yReset = 1.0f;  // Y-axis reset position
    public float zReset = 0.0f;  // Z-axis reset position

     private void OnEnable()
    {
        currentJumpAttempt = new JumpAttempt(3,attemptNumber, 0, 0, 0, 0, 0, 0, 0, 0, 0, date: System.DateTime.Now);
        uiManager.DisableHudElements();
        uiManager.DisableStatScreen();
        uiManager.EnableStartElements();
        playerManager.ResetPlayer(xReset, yReset, zReset);
    }

    public void startAttempt()
    {
        uiManager.EnableHudElements();
        uiManager.DisableStatScreen();
        uiManager.DisableStartElements();
        surfCharacter.movementEnabled = true;
        playerManager.EnableMouseLook();
        if(!allowPlayerMovement)
        {
            surfCharacter.controller.moveForward = true;
            //has to jump
        }
        mouseAngleTracker.isAttemptActive = true;
        speedTracker.isAttemptActive = true;
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
        float score = speedTracker.CalculateAttemptSpeed() + (20 - System.Math.Abs(45 - mouseAngleTracker.CalculateAttemptAngleChange())); //+ mouseAngleTracker.CalculateAverageAttemptAngleSmoothness();//(10 - Math.Abs(mouseAngleTracker.CalculateAverageAttemptAngleSmoothness()));
        currentJumpAttempt = new JumpAttempt(3, attemptNumber, 0, 0, 0, 0, speedTracker.CalculateAttemptSpeed(), score, mouseAngleTracker.CalculateAttemptAngleChange(), mouseAngleTracker.CalculateAverageAttemptAngleSmoothness(), 0, date: System.DateTime.Now);
        scoreManager.SaveScore(currentJumpAttempt);
        //TODO: stats are going to be different depending on the scene, this should probably be dont in the scene manager but I dont know
        //jank, fix later
        uiManager.StatScreen.GetComponent<StatScreen>().currentJumpAttempt = currentJumpAttempt;
        uiManager.StatScreen.GetComponent<StatScreen>().lastJumpAttempt = lastJumpAttempt;
        uiManager.StatScreen.GetComponent<StatScreen>().updateStats();
        hasJumped = false;
        mouseAngleTracker.isAttemptActive = false;
        speedTracker.isAttemptActive = false;
        lastJumpAttempt = currentJumpAttempt;
        
        
    }

    public void resetScene()
    {
        playerManager.ResetPlayer(xReset, yReset, zReset);
        uiManager.DisableHudElements();
        uiManager.EnableStatScreen();
        uiManager.EnableStartElements();
        startTriggered = false;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator handleJump()
    {
        for(int i = 0; i < 10; i++)
        {
            yield return null;
        }
        hasJumped = true;
    }

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
        //check if the player crosses the start line
        if(surfCharacter.transform.position.z <= 0.05 && surfCharacter.transform.position.z >= -0.05 && !startTriggered && !allowPlayerMovement)
        {
            Debug.Log("Reset");
            surfCharacter.controller.moveForward = false;
            surfCharacter.controller.attemptWishJump = true;
            surfCharacter.controller.moveRight = true;
            StartCoroutine(handleJump());
            startTriggered = true;
        }
            
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
        if (hasJumped == true && grounded == true)
        {
            endAttempt();
            resetScene();
        }
    }
}
