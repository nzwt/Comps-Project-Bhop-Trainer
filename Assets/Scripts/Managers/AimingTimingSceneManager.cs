using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fragsurf.Movement;

public class AimingTimingSceneManager : MonoBehaviour
{
 public SurfCharacter surfCharacter;
    public UIManager uiManager;
    public PlayerManager playerManager;
    public ScoreManager scoreManager;
    public JumpIndicator jumpIndicator;
    public MouseAngleTracker mouseAngleTracker;
    public OrbController orbController;

    // game objects
    public JumpAttempt currentJumpAttempt;
    public JumpAttempt lastJumpAttempt;
    public int attemptNumber = 0;
    public GameObject cameraHolder;
    
    // managment bools
    public bool lastScoreLoaded = false;
    public bool startTriggered = false;
    public bool playerStart = false;
    //management values
    public int maxSwitches = 5;
    public List<float> switchTimes = new List<float>();
    private float switchTimer = -1;
    private float globalTimer = -1;
    //0 is left, 1 is right
    public Transform currentTarget;
    public Transform leftTarget;
    public Transform rightTarget;

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
        currentTarget = leftTarget;
    }

    public void startAttempt()
    {
        ///text appears: move mouse to either the left or the right to start an attempt, then switch targets by smoothly moving your mouse to the other target each time the 
        /// indicator changes color.
        uiManager.EnableHudElements();
        uiManager.DisableStatScreen();
        uiManager.DisableStartElements();
        surfCharacter.movementEnabled = false;
        playerManager.EnableMouseLook();
        currentTarget = leftTarget;
        mouseAngleTracker.isAttemptActive = true;
        orbController.TargetLeft();
    }

    public void endAttempt()
    {
        playerManager.DisableMouseLook();
        // Save the score, values are placeholders for now
        attemptNumber++;
        // Update the lastJumpAttempt to the currentJumpAttempt
        // Reset the currentJumpAttempt
        float score = 0;
        currentJumpAttempt = new JumpAttempt(1,attemptNumber, 0, 0, 0, 0, 0, score, 0, 0, 0, date: System.DateTime.Now);
        scoreManager.SaveScore(currentJumpAttempt);
        //TODO: stats are going to be different depending on the scene, this should probably be dont in the scene manager but I dont know
        //jank, fix later
        uiManager.StatScreen.GetComponent<BhopStatScreen>().currentJumpAttempt = currentJumpAttempt;
        uiManager.StatScreen.GetComponent<BhopStatScreen>().lastJumpAttempt = lastJumpAttempt;
        uiManager.StatScreen.GetComponent<BhopStatScreen>().updateStats();
        //reset vars
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

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {

        //load the scores
        if(scoreManager.isLoaded == true && lastScoreLoaded == false && scoreManager.GetLastJumpAttempt() != null)
        {

            //load the most recent score
            lastJumpAttempt = scoreManager.GetLastJumpAttempt();
            attemptNumber = lastJumpAttempt.attemptNumber + 1;
            lastScoreLoaded = true; 
        }

        //CheckAimAccuracy();

        //IN RUN LOGIC//
        //start attempt by moving over left orb
        if(playerStart == false)
        {
            if(mouseAngleTracker.angleChange < 0)
            {
                if (mouseAngleTracker.angleChange < -20 && mouseAngleTracker.angleChange > -25)
                {
                    playerStart = true;
                    //Debug.Log("Player is looking left");
                    switchTimer = 0;
                    orbController.TargetRight();
                }
            }
        }
        //if attempt is active, add time
        if(playerStart == true)
        {
            switchTimer += Time.deltaTime;
        }
        //check if the player has started the attempt, if not, are they looking at a target?
        if(playerStart == true)
        {
            if(mouseAngleTracker.angleChange > 0)
            {
                if(mouseAngleTracker.angleChange > 20 && mouseAngleTracker.angleChange < 25 && currentTarget == rightTarget)
                {
                    currentTarget = leftTarget;
                    switchTimes.Add(switchTimer);
                    switchTimer = 0;
                    orbController.TargetLeft();
                    //Debug.Log("Player is looking right");
                }
            }
            else if(mouseAngleTracker.angleChange < 0)
            {
                if (mouseAngleTracker.angleChange < -20 && mouseAngleTracker.angleChange > -25 && currentTarget == leftTarget)
                {
                    currentTarget = rightTarget;
                    switchTimes.Add(switchTimer);
                    switchTimer = 0;
                    orbController.TargetRight();
                    //Debug.Log("Player is looking left");
                }
            }
        }

        //TODO - need to update this to have checks for starting level
        if (Input.GetMouseButtonDown(0))
        {
            startAttempt();
        }

        if (switchTimes.Count >= maxSwitches)
        {
            // Player has reached max switches, end the attempt
            foreach (float time in switchTimes)
            {
                Debug.Log(time);
            }
            endAttempt();
            resetScene();
        }

    }
}