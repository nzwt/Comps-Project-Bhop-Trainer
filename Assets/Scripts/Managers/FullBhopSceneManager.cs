using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fragsurf.Movement;
using System;
using System.Linq;

public class FullBhopSceneManager : MonoBehaviour
{
 public SurfCharacter surfCharacter;
    public UIManager uiManager;
    public PlayerManager playerManager;
    public ScoreManager scoreManager;
    public JumpIndicator jumpIndicator;
    public MouseAngleTracker mouseAngleTracker;
    public SpeedTracker speedTracker;
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
    public int maxSwitches = 6;
    public List<float> switchTimes = new List<float>();
    public List<float> rightLookTimes = new List<float>();
    public List<float> leftLookTimes = new List<float>();
    public float[] APressedTimestamps ;
    public float[] DPressedTimestamps ;
    [SerializeField]
    public float[] APressedTimes ;
    [SerializeField]
    public float[] DPressedTimes ;
    public float[] APressedOffset ;
    public float[] DPressedOffset ;
    public float[] AReleasedOffset ;
    public float[] DReleasedOffset ;
    private float ATimer = -1;
    private float DTimer = -1;
    private float switchTimer = -1;
    private float globalTimer = -1;
    [SerializeField]
    private int rightSwitchCount = -1;
    [SerializeField]
    private int leftSwitchCount = -1;
    //false is left, true is right
    public bool lookDirection = false;

    // Jumping Variables
    public bool hasJumped = false;
    public bool firstFrame = true;
    public bool allowPlayerMovement = true;
    // jump management values
    public int maxJumps = 5;
    private int currentJumps = 0;
    public List<float> groundTimes = new List<float>();
    private float groundTimer = -1;
    
    //jumpAttempt stats
    private float bhopAccuracy = 0;
    private float strafeTimingOffset = 0;
    private int scorePenalties = 0;

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
        
        resetArrays();
    }

    public void startAttempt()
    {
        ///text appears: move mouse to either the left or the right to start an attempt, then switch targets by smoothly moving your mouse to the other target each time the 
        /// indicator changes color.
        /// HUD
        uiManager.EnableHudElements();
        uiManager.DisableStatScreen();
        uiManager.DisableStartElements();
        //Player control enabled
        surfCharacter.movementEnabled = false;
        playerManager.EnableMouseLook();
        //make targets virtual
        lookDirection = true;
        mouseAngleTracker.resetAngleChange();
        mouseAngleTracker.isAttemptActive = true;
        arrow.transform.rotation = Quaternion.Euler(0, 180, 0);
        speedTracker.isAttemptActive = true;
        orbController.resetTargets();
        bhopAccuracy = 0;
        scorePenalties = 0;
    }

    public void endAttempt()
    {
        playerManager.DisableMouseLook();
        // Save the score, values are placeholders for now
        attemptNumber++;
        // Update the lastJumpAttempt to the currentJumpAttempt
        // Reset the currentJumpAttempt
        float score = (0.65f - Math.Abs(bhopAccuracy))*7.65f + (0.65f - Math.Abs(strafeTimingOffset))*7.65f;
        currentJumpAttempt = new JumpAttempt(4,attemptNumber, strafeTimingOffset, 0, 0, 0, 0, score, 0, 0, bhopAccuracy, date: System.DateTime.Now);
        scoreManager.SaveScore(currentJumpAttempt);
        //TODO: stats are going to be different depending on the scene, this should probably be dont in the scene manager but I dont know
        //jank, fix later
        uiManager.StatScreen.GetComponent<StrafingStatScreen>().currentJumpAttempt = currentJumpAttempt;
        uiManager.StatScreen.GetComponent<StrafingStatScreen>().lastJumpAttempt = lastJumpAttempt;
        uiManager.StatScreen.GetComponent<StrafingStatScreen>().updateStats();
        //reset vars
        lastJumpAttempt = currentJumpAttempt;
        surfCharacter.moveData.velocity = Vector3.zero;
        surfCharacter.moveData.wishJump = false;
        switchTimes.Clear();
        jumpIndicator.deleteLines();
        playerStart = false;
        mouseAngleTracker.isAttemptActive = false;
        resetArrays();
        
        
    }

    public void resetArrays()
    {
        APressedTimestamps = Enumerable.Repeat(-1000f, maxSwitches).ToArray();
        DPressedTimestamps = Enumerable.Repeat(-1000f, maxSwitches).ToArray();
        APressedTimes = Enumerable.Repeat(-1000f, maxSwitches).ToArray();
        DPressedTimes = Enumerable.Repeat(-1000f, maxSwitches).ToArray();
        APressedOffset = Enumerable.Repeat(-1000f, maxSwitches).ToArray();
        DPressedOffset = Enumerable.Repeat(-1000f, maxSwitches).ToArray();
        AReleasedOffset = Enumerable.Repeat(-1000f, maxSwitches).ToArray();
        DReleasedOffset = Enumerable.Repeat(-1000f, maxSwitches).ToArray();
        rightLookTimes = new List<float>();
        leftLookTimes = new List<float>();
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
        //start attempt by crossing start line
        if(playerStart == false)
        {
            if(surfCharacter.transform.position.z <= 1.2 && surfCharacter.transform.position.z >= 1)
            {
                playerStart = true;
                mouseAngleTracker.resetAngleChange();
                rightSwitchCount = 0;
                leftSwitchCount = -1;
                ATimer = 0;
                DTimer = 0;
                //You should look right first
                lookDirection = true;
            }
            if(mouseAngleTracker.angleChange < 0)
            {
                if (mouseAngleTracker.angleChange < -20 && mouseAngleTracker.angleChange > -25)
                {
                    playerStart = true;
                    //Debug.Log("Player is looking left");
                    switchTimer = 0;
                    globalTimer = 0;
                    rightSwitchCount = 0;
                    leftSwitchCount = -1;
                    ATimer = 0;
                    DTimer = 0;
                    lookDirection = true;
                    orbController.TargetLeft();
                    arrow.transform.rotation = Quaternion.Euler(0, 0, 0);
                    jumpIndicator.StartJump();
                    rightLookTimes.Add(globalTimer);
                }
            }
        }

        /// Jump Tracking
        bool grounded = true;
        //skip first frame, otherwise the player will register an attempt
        if(surfCharacter.groundObject == null)
        {
            grounded = false;
        }

        if (hasJumped == false && grounded == false)
        {
            // Player has jumped, register the jump
            //print("Jumped");
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
            //check timer and assign a color based on performance
            if(groundTimes[groundTimes.Count - 1] > 0.1)
            {
                jumpIndicator.changeLastLineColor(Color.red);
            }
            else if(groundTimes[groundTimes.Count - 1] > 0.05)
            {
                jumpIndicator.changeLastLineColor(Color.yellow);
            }
            else
            {
                jumpIndicator.changeLastLineColor(Color.green);
            }
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


        //if attempt is active, add time
        if(playerStart == true)
        {
            globalTimer += Time.deltaTime;
            switchTimer += Time.deltaTime;
            //TODO: Bug - if the player holds D then lets go and taps A a bunch of times before looking left, it makes the array too big
            if(Input.GetKeyDown(KeyCode.A)  && APressedTimestamps[leftSwitchCount] == -1000)//&& DHeld == false)
            {
                AHeld = true;
                APressedTimestamps[leftSwitchCount] = (globalTimer);
                // Debug.Log("A pressed");
                // Debug.Log(globalTimer);
            }
            if(Input.GetKeyDown(KeyCode.D) && DPressedTimestamps[rightSwitchCount] == -1000)//&& AHeld == false)
            {
                DHeld = true;
                DPressedTimestamps[rightSwitchCount] = (globalTimer);
                // Debug.Log("D pressed");
            }
            if(Input.GetKey(KeyCode.A) && AHeld == true)
            {
                ATimer += Time.deltaTime;
            }
            if(Input.GetKey(KeyCode.D) && DHeld == true)
            {
                DTimer += Time.deltaTime;
            }
            if(Input.GetKeyUp(KeyCode.A) && AHeld == true)
            {
                AHeld = false;
                APressedTimes[leftSwitchCount] = ATimer;
                ATimer = 0;
                // Debug.Log("A released");
            }
            if(Input.GetKeyUp(KeyCode.D) && DHeld == true)
            {
                DHeld = false;
                DPressedTimes[rightSwitchCount] = DTimer;
                DTimer = 0;
                // Debug.Log("D released");
            }
        }
        //check if the player has started the attempt, if not, are they looking at a target?
        if(playerStart == true)
        {
            if(mouseAngleTracker.angleChange > 0)
            {
                if(mouseAngleTracker.angleChange > 20 && mouseAngleTracker.angleChange < 25 && lookDirection == true && switchTimer > -1)
                {
                    switchTimes.Add(switchTimer);
                    orbController.ShowRightAccuracy(switchTimer);
                    switchTimer = 0;
                    arrow.transform.Rotate(0, 180, 0);
                    lookDirection = false;
                    jumpIndicator.StartJump();
                    leftLookTimes.Add(globalTimer);
                    leftSwitchCount++;
                    //Debug.Log("Player is looking right");
                }
            }
            else if(mouseAngleTracker.angleChange < 0)
            {
                if (mouseAngleTracker.angleChange < -20 && mouseAngleTracker.angleChange > -25 && lookDirection == false && switchTimer > -1)
                {
                    switchTimes.Add(switchTimer);
                    orbController.ShowLeftAccuracy(switchTimer);
                    switchTimer = 0;
                    arrow.transform.Rotate(0, 180, 0);
                    lookDirection = true;
                    jumpIndicator.StartJump();
                    rightLookTimes.Add(globalTimer);
                    rightSwitchCount++;
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
            //If the player did not let go of the key, end the time
            if(AHeld == true)
            {
                APressedTimes[leftSwitchCount] = ATimer;
            }
            else if(DHeld == true)
            {
                DPressedTimes[rightSwitchCount] = DTimer;
            }
            //calculate look accuracy
            float endTime = globalTimer;
            foreach (float time in switchTimes)
            {
                bhopAccuracy += time - 0.65f;
            }
            bhopAccuracy = bhopAccuracy / maxSwitches;
            //calculate strafe accuracy
            //calculate offset of D press from switch

            //calculate offset of all D presses from switch
            for(int i = 0; i < rightLookTimes.Count; i++)
            {
                if(DPressedTimestamps[i] < rightLookTimes[i] && DPressedTimestamps[i] > rightLookTimes[i] - 0.2f)
                {
                    DPressedOffset[i] = rightLookTimes[i] - DPressedTimestamps[i];
                }
                else if(DPressedTimestamps[i] > rightLookTimes[i] && DPressedTimestamps[i] < rightLookTimes[i] + 0.2f)
                {
                    DPressedOffset[i] = DPressedTimestamps[i] - rightLookTimes[i];
                }               
            }
            //calculate offset of all D releases from switch
            for(int i = 0; i < rightLookTimes.Count; i++)
            {
                float release = (DPressedOffset[i] + rightLookTimes[i]) + DPressedTimes[i];
                if(release < leftLookTimes[i] && release > leftLookTimes[i] - 0.2f)
                {
                    DReleasedOffset[i] = leftLookTimes[i] - release;
                }
                else if(release > leftLookTimes[i] && release < leftLookTimes[i] + 0.2f)
                {
                    DReleasedOffset[i] = release - leftLookTimes[i];
                }
            }

            //calculate offset of all A presses from switch
            for(int i = 0; i < leftLookTimes.Count; i++)
            {
                if(APressedTimestamps[i] < leftLookTimes[i] && APressedTimestamps[i] > leftLookTimes[i] - 0.2f)
                {
                    APressedOffset[i] = leftLookTimes[i] - APressedTimestamps[i];
                }
                else if(APressedTimestamps[i] > leftLookTimes[i] && APressedTimestamps[i] < leftLookTimes[i] + 0.2f)
                {
                    APressedOffset[i] = APressedTimestamps[i] - leftLookTimes[i];
                }
            }
            //calculate offset of all A releases from switch
            for(int i = 0; i < leftLookTimes.Count; i++)
            {
                float release = (APressedOffset[i] + leftLookTimes[i]) + APressedTimes[i];
                if(release < rightLookTimes[i] && release > rightLookTimes[i] - 0.2f)
                {
                    AReleasedOffset[i] = rightLookTimes[i] - release;
                }
                else if(release > rightLookTimes[i] && release < rightLookTimes[i] + 0.2f)
                {
                    AReleasedOffset[i] = release - rightLookTimes[i] ;
                }
            }

          



            //calculate strafe accuracy
            Debug.Log("A Held Accuracy");
            float totalOffset = 0;
            int totalCounted = 0;
            for(int i = 0; i < leftLookTimes.Count; i++)
            {
                Debug.Log(APressedOffset[i]);
                Debug.Log(AReleasedOffset[i]);
                if(APressedOffset[i] != -1000)
                {
                    totalOffset += APressedOffset[i];
                    totalCounted += 1;
                }
                else
                {
                    scorePenalties += 1;
                }
                //if they missed by too much, apply a score penalty but dont change the accuracy timing
                //TODO: make a stat for total misses?
                if(AReleasedOffset[i] != -1000)
                {
                    totalOffset += AReleasedOffset[i];
                    totalCounted += 1;
                }
                else
                {
                    scorePenalties += 1;
                }
            }

            Debug.Log("D Held Accuracy");
            for(int i = 0; i < rightLookTimes.Count; i++)
            {
                Debug.Log(DPressedOffset[i]);
                Debug.Log(DReleasedOffset[i]);
                if(DPressedOffset[i] != -1000)
                {
                    totalOffset += DPressedOffset[i];
                    totalCounted += 1;
                }
                else
                {
                    scorePenalties += 1;
                }
                if(DReleasedOffset[i] != -1000)
                {
                    totalOffset += DReleasedOffset[i];
                    totalCounted += 1;
                }
                else
                {
                    scorePenalties += 1;
                }
            }
            strafeTimingOffset = totalOffset / totalCounted;

            //calculate timing offset
            foreach (float time in switchTimes)
            {
                bhopAccuracy += time - 0.65f;
            }
            bhopAccuracy = bhopAccuracy / maxSwitches;


            //for each switch, check from halfway to last switch to halfway to next switch and determine accuracy

            endAttempt();
            resetScene();
        }

    }
}

