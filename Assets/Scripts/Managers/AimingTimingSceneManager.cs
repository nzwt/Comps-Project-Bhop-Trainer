using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fragsurf.Movement;
using System;

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
    public GameObject arrow; //flip x to aim left
    
    // managment bools
    public bool lastScoreLoaded = false;
    public bool startTriggered = false;
    public bool playerStart = false;
    //management values
    public int maxSwitches = 5;
    public List<float> switchTimes = new List<float>();
    private float switchTimer = -1;
    private float globalTimer = -1;
    private float switchTrackTimer = 0;
    private float switchTrackStart = 0;
    private float switchTrackEnd = 0;
    //0 is left, 1 is right
    public Transform currentTarget;
    public Transform leftTarget;
    public Transform rightTarget;
    private float bhopAccuracy = 0;

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
        mouseAngleTracker.resetAngleChange();
        mouseAngleTracker.isAttemptActive = true;
        arrow.transform.rotation = Quaternion.Euler(0, 0, 0);
        orbController.resetTargets();
        bhopAccuracy = 0;
    }

    public void endAttempt()
    {
        playerManager.DisableMouseLook();
        // Save the score, values are placeholders for now
        attemptNumber++;
        // Update the lastJumpAttempt to the currentJumpAttempt
        // Reset the currentJumpAttempt
        float score = (0.65f - Math.Abs(bhopAccuracy))*15.4f;
        currentJumpAttempt = new JumpAttempt(3,attemptNumber, 0, 0, 0, 0, 0, score, 0, 0, bhopAccuracy, date: System.DateTime.Now);
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
        switchTimes.Clear();
        jumpIndicator.deleteLines();
        playerStart = false;
        mouseAngleTracker.isAttemptActive = false;
        
        
    }



    public void resetScene()
    {
        playerManager.ResetPlayer(xReset, yReset, zReset);
        uiManager.DisableHudElements();
        uiManager.EnableStatScreen();
        uiManager.EnableStartElements();
        startTriggered = false;
    }

    // IEnumerator SwitchTargetsOnTime()
    // {
    //     while (true)
    //     {
    //         Debug.Log("Switching targets");
    //         orbController.switchTarget();
    //         switchTimer = 0;
    //         if(currentTarget == leftTarget)
    //         {
    //             currentTarget = rightTarget;
    //         }
    //         else
    //         {
    //             currentTarget = leftTarget;
    //         }
    //         // Wait for 0.65 seconds before executing the next loop
    //         yield return new WaitForSeconds(0.65f);
    //     }
    // }
    
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
                    currentTarget = rightTarget;
                    orbController.TargetLeft();
                    arrow.transform.rotation = Quaternion.Euler(0, 0, 0);
                    jumpIndicator.StartJump();
                    switchTrackStart = mouseAngleTracker.angleChange;
                }
            }
        }
        //if attempt is active, add time
        if(playerStart == true)
        {
            switchTimer += Time.deltaTime;
            if(switchTrackTimer < 0.05)
            {
                switchTrackTimer += Time.deltaTime;
            }
            else
            {
                switchTrackEnd = mouseAngleTracker.angleChange;
                if(currentTarget == leftTarget)
                {
                    if( switchTrackEnd  > switchTrackStart + 3)
                    {
                        Debug.Log("going right");
                        currentTarget = rightTarget;
                    }
                }
                else if(currentTarget == rightTarget)
                {
                    if( switchTrackEnd  < switchTrackStart - 3)
                    {
                        Debug.Log("going left");
                        currentTarget = leftTarget;
                    }
                }
                switchTrackTimer = 0;
                switchTrackStart = switchTrackEnd;
            }
            
        }
        //check if the player has started the attempt, if not, are they looking at a target?
        if(playerStart == true)
        {
            if(mouseAngleTracker.angleChange > 0)
            {
                if(mouseAngleTracker.angleChange > 20 && mouseAngleTracker.angleChange < 25 && currentTarget == rightTarget && switchTimer > -1)
                {
                    switchTimes.Add(switchTimer);
                    orbController.ShowRightAccuracy(switchTimer);
                    switchTimer = 0;
                    arrow.transform.Rotate(0, 180, 0);
                    currentTarget = leftTarget;
                    jumpIndicator.StartJump();
                    //Debug.Log("Player is looking right");
                }
            }
            else if(mouseAngleTracker.angleChange < 0)
            {
                if (mouseAngleTracker.angleChange < -20 && mouseAngleTracker.angleChange > -25 && currentTarget == leftTarget && switchTimer > -1)
                {
                    switchTimes.Add(switchTimer);
                    orbController.ShowLeftAccuracy(switchTimer);
                    switchTimer = 0;
                    arrow.transform.Rotate(0, 180, 0);
                    currentTarget = rightTarget;
                    jumpIndicator.StartJump();
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
                bhopAccuracy += time - 0.65f;
            }
            bhopAccuracy = bhopAccuracy / maxSwitches;
            endAttempt();
            resetScene();
        }

    }
}