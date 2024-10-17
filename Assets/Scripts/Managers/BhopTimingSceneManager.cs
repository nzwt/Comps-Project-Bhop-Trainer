using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fragsurf.Movement;

public class BhopTimingSceneManager : MonoBehaviour
{// refrences to other scripts
    public SurfCharacter surfCharacter;
    public UIManager uiManager;
    public PlayerManager playerManager;
    public ScoreManager scoreManager;
    public JumpIndicator jumpIndicator;

    // game objects
    public JumpAttempt currentJumpAttempt;
    public JumpAttempt lastJumpAttempt;
    public int attemptNumber = 0;
    public SpeedTracker speedTracker;
    
    // managment bools
    public bool hasJumped = false;
    public bool firstFrame = true;
    public bool lastScoreLoaded = false;
    public bool startTriggered = false;
    public bool allowPlayerMovement = true;
    //management values
    public int maxJumps = 5;
    private int currentJumps = 0;
    public List<float> groundTimes = new List<float>();
    private float groundTimer = -1;

    // Reset position values
    public float xReset = 0.0f;  // X-axis reset position
    public float yReset = 1.0f;  // Y-axis reset position
    public float zReset = 0.0f;  // Z-axis reset position

     private void OnEnable()
    {
        currentJumpAttempt = new JumpAttempt(1,attemptNumber, 0, 0, 0, 0, 0, 0, 0, 0, 0, date: System.DateTime.Now);
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
        surfCharacter.DisableNonJumpInput();
        StartCoroutine(allowJump());
        //playerManager.EnableMouseLook();
        surfCharacter.controller.moveForward = true;
        speedTracker.isAttemptActive = true;
        startTriggered = true;
    }

    public void endAttempt()
    {
        //stop the movement
        surfCharacter.controller.moveForward = false;
        // Save the score, values are placeholders for now
        attemptNumber++;
        // Update the lastJumpAttempt to the currentJumpAttempt
        // Reset the currentJumpAttempt
        float score = speedTracker.CalculateAttemptSpeed() + (1-calculateBhopAccuracy())*10;
        currentJumpAttempt = new JumpAttempt(1,attemptNumber, 0, 0, 0, 0, speedTracker.CalculateAttemptSpeed(), score, 0, 0, calculateBhopAccuracy(), date: System.DateTime.Now);
        scoreManager.SaveScore(currentJumpAttempt);
        //TODO: stats are going to be different depending on the scene, this should probably be dont in the scene manager but I dont know
        //jank, fix later
        uiManager.StatScreen.GetComponent<BhopStatScreen>().currentJumpAttempt = currentJumpAttempt;
        uiManager.StatScreen.GetComponent<BhopStatScreen>().lastJumpAttempt = lastJumpAttempt;
        uiManager.StatScreen.GetComponent<BhopStatScreen>().updateStats();
        //reset vars
        jumpIndicator.deleteLines();
        currentJumps = 0;
        groundTimes.Clear();
        hasJumped = false;
        speedTracker.isAttemptActive = false;
        lastJumpAttempt = currentJumpAttempt;
        surfCharacter.moveData.velocity = Vector3.zero;
        surfCharacter.moveData.wishJump = false;
        
        
    }

    public void resetScene()
    {
        playerManager.ResetPlayer(xReset, yReset, zReset);
        uiManager.DisableHudElements();
        uiManager.EnableStatScreen();
        uiManager.EnableStartElements();
        startTriggered = false;
    }

    public float calculateBhopAccuracy()
    {
        if (groundTimes.Count == 0)
        {
            return 0f;
        }

        float sum = 0f;
        foreach (float time in groundTimes)
        {
            sum += time;
        }

        return sum / groundTimes.Count;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator allowJump()
    {
        for(int i = 0; i < 10; i++)
        {
            yield return null;
        }
        surfCharacter.movementEnabled = true;
    }

    void Update()
    {   bool grounded = true;
        //skip first frame, otherwise the player will register an attempt
        if(surfCharacter.groundObject == null)
        {
            grounded = false;
        }
        if(firstFrame)
        {
            grounded = true;
            firstFrame = false;
            surfCharacter.moveData.wishJump = false;
        }
        //load the scores
        if(scoreManager.isLoaded == true && lastScoreLoaded == false && scoreManager.GetLastJumpAttempt() != null)
        {

            //load the most recent score
            lastJumpAttempt = scoreManager.GetLastJumpAttempt();
            attemptNumber = lastJumpAttempt.attemptNumber + 1;
            lastScoreLoaded = true; 
        }

        //IN RUN LOGIC//
        //check if the player crosses the line (maybe make this a trigger) (should this be the end of the attempt?)
        Debug.Log(surfCharacter.transform.position.z);
        if(surfCharacter.transform.position.z <= 27.5 && surfCharacter.transform.position.z >= 27 && startTriggered == true)
        {
            endAttempt();
            resetScene();
        }
        //calculate how long the player has been on the ground

        //TODO - need to update this to have checks for starting level
        if (Input.GetMouseButtonDown(0))
        {
            startAttempt();
        }
        if (hasJumped == false && grounded == false)
        {
            // Player has jumped, register the jump
            print("Jumped");
            hasJumped = true;
            jumpIndicator.StartJump();
        }

        if (hasJumped == true && grounded == true && groundTimer == -1)
        {
            // Player landed after a jump, start timing the grounded period
            groundTimer = 0;
            hasJumped = false; // Reset jump status after landing
            jumpIndicator.EndJump();
        }

        if (hasJumped == false && grounded == true && groundTimer >= 0)
        {
            // Player is on the ground, increment ground time
            groundTimer += Time.deltaTime;
        }

        if (hasJumped == true && grounded == false && groundTimer >= 0)
        {
            // Player has jumped, add the ground time to the list
            groundTimes.Add(groundTimer);
            //Debug.Log("Ground time recorded: " + groundTimer);
            currentJumps++;
            //Debug.Log("Current jumps: " + currentJumps);
            groundTimer = -1; // Reset timer after recording
        }

        if (currentJumps >= maxJumps)
        {
            // Player has reached max jumps, end the attempt
            foreach (float time in groundTimes)
            {
                //Debug.Log(time);
            }
            endAttempt();
            resetScene();
        }

        // if( hasJumped == false && grounded == false)
        // {
        //     print("Jumped");
        //     hasJumped = true;
        // }
        // if (hasJumped == true && grounded == true && currentJumps == maxJumps)
        // {
        //     foreach (float time in groundTimes)
        //     {
        //         Debug.Log(time);
        //     }
        //     //Debug.Log(groundTimes);
        //     endAttempt();
        //     resetScene();
        // }
        // if (hasJumped == true && grounded == true && groundTimer == -1)
        // {
        //     currentJumps++;
        //     groundTimer = 0;
        //     hasJumped = false;
        // }
        // else if (hasJumped == false && grounded == true && groundTimer >= 0)
        // {
        //     groundTimer += Time.deltaTime;
        // }
        // else if (hasJumped == true && grounded == false)
        // {
        //     groundTimes.Add(groundTimer);
        //     groundTimer = -1;
        // }

    }
}
