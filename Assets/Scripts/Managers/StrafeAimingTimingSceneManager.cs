using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fragsurf.Movement;
using System;
using System.Linq;

public class StrafeAimingTimingSceneManager : MonoBehaviour
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
    private bool AHeld = false;
    private bool DHeld = false;
    //management values
    public int maxSwitches = 3;
    public List<float> switchTimes = new List<float>();
    public List<float> rightLookTimes = new List<float>();
    public List<float> leftLookTimes = new List<float>();
    public List<float> APressedTimes = new List<float>();
    public List<float> DPressedTimes = new List<float>();
    public List<float> AReleasedTimes= new List<float>();
    public List<float> DReleasedTimes = new List<float>();
    public float[] AHeldAccuracy = Enumerable.Repeat(-1000f, 6).ToArray();
    public float[] DHeldAccuracy = Enumerable.Repeat(-1000f, 6).ToArray();
    private float switchTimer = -1;
    private float globalTimer = -1;
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
                    globalTimer = 0;
                    currentTarget = rightTarget;
                    orbController.TargetLeft();
                    arrow.transform.rotation = Quaternion.Euler(0, 0, 0);
                    jumpIndicator.StartJump();
                    rightLookTimes.Add(globalTimer);
                }
            }
        }
        //if attempt is active, add time
        if(playerStart == true)
        {
            globalTimer += Time.deltaTime;
            switchTimer += Time.deltaTime;
            if(Input.GetKeyDown(KeyCode.A) )//&& DHeld == false)
            {
                AHeld = true;
                APressedTimes.Add(globalTimer);
                Debug.Log("A pressed");
                Debug.Log(globalTimer);
            }
            if(Input.GetKeyDown(KeyCode.D) )//&& AHeld == false)
            {
                DHeld = true;
                DPressedTimes.Add(globalTimer);
                Debug.Log("D pressed");
            }
            if(Input.GetKeyUp(KeyCode.A))
            {
                AHeld = false;
                AReleasedTimes.Add(globalTimer);
            }
            if(Input.GetKeyUp(KeyCode.D))
            {
                DHeld = false;
                DReleasedTimes.Add(globalTimer);
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
                    leftLookTimes.Add(globalTimer);
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
                    rightLookTimes.Add(globalTimer);
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
            //calculate look accuracy
            if(DReleasedTimes.Count < 3)
            {
                DReleasedTimes.Add(globalTimer);
            }
            foreach (float time in switchTimes)
            {
                bhopAccuracy += time - 0.65f;
            }
            bhopAccuracy = bhopAccuracy / maxSwitches;
            //calculate strafe accuracy
            //for first switch, check from global start to halfway to next attempt
            if(DPressedTimes[0] < rightLookTimes[0] && DPressedTimes[0] > 0 )
            {
                DHeldAccuracy[0] = rightLookTimes[0] - DPressedTimes[0];
            }
            else if(DPressedTimes[0] > rightLookTimes[0] && DPressedTimes[0] < leftLookTimes[0])
            {
                DHeldAccuracy[0] = DPressedTimes[0] - rightLookTimes[0];
            }
            //maybe i want a continous hold check to make sure this doesn't mess up and overwrite

            //calculate offset of start of D press from switch
            for(int i = 2; i < 6; i +=2)
            {
                for(int j = 1; j < DPressedTimes.Count; j++)
                {
                    if(DHeldAccuracy[i] != -1000)
                    {
                        continue;
                    }
                    float time = DPressedTimes[j];
                    if(time < rightLookTimes[i/2] && time > leftLookTimes[(i/2)-1] && DHeldAccuracy[i] == -1000)
                    {
                        DHeldAccuracy[i] = rightLookTimes[i/2] - time;
                    }
                    else if(time > rightLookTimes[i/2] && time < leftLookTimes[i/2] && DHeldAccuracy[i] == -1000)
                    {
                        DHeldAccuracy[i] = time-rightLookTimes[i/2];
                    }
                }
            }

            
            //calculate offset of end of A press from switch
            for(int i = 1; i < 6; i +=2)
            {
                for(int j = 0; j < DReleasedTimes.Count; j++)
                {
                    if(DHeldAccuracy[i] != -1000)
                    {
                        continue;
                    }
                    float time = DReleasedTimes[j]; //if they missed the time by more than 0.2 seconds, they need some work
                    if(time < leftLookTimes[i/2] && time > (leftLookTimes[(i/2)] - 0.2) && DHeldAccuracy[i] == -1000)
                    {
                        DHeldAccuracy[i] = leftLookTimes[i/2] - time;
                    }
                    else if(time > leftLookTimes[i/2] && time < (leftLookTimes[(i/2)] + 0.2) && DHeldAccuracy[i] == -1000)
                    {
                        DHeldAccuracy[i] = time-leftLookTimes[i/2];
                    }
                }
            }

            //calculate strafe accuracy
            Debug.Log("D Held Accuracy");
            for(int i = 0; i < 6; i++)
            {
                Debug.Log(DHeldAccuracy[i]);
            }


            //for each switch, check from halfway to last switch to halfway to next switch and determine accuracy

            endAttempt();
            resetScene();
        }

    }
}
