using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fragsurf.Movement;
using System;
using System.Linq;
using UnityEngine.Analytics;

public class FullBhopSceneManager : MonoBehaviour
{
 public SurfCharacter surfCharacter;
    public UIManager uiManager;
    public PlayerManager playerManager;
    public ScoreManager scoreManager;
    public ArcJumpIndicator jumpIndicator;
    public MouseAngleTracker mouseAngleTracker;
    public SpeedTracker speedTracker;
    public TimelineController timelineController;
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
    //Look vars
    bool firstTime = true;
    public int maxSwitches = 6;
    public List<float> switchTimes = new List<float>();
    public List<float> rightLookTimes = new List<float>();
    public List<float> leftLookTimes = new List<float>();
    public float[] jumpTimestamps;
    private float rightAngleStart = 0;
    private float leftAngleStart = 0;
    private int switchCount = 0;
    private float switchTrackTimer = 0;
    private float switchTrackStart = 0;
    private float switchTrackEnd = 0;

    //Strafe Variables
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
    public bool firstJump = true;
    public bool allowPlayerMovement = true;
    // jump management values
    public int maxJumps = 5;
    private int currentJumps = 0;
    public List<float> groundTimes = new List<float>();
    private float groundTimer = -1;
    
    //jumpAttempt stats
    private float bhopAccuracy = 0;
    private float lookOffset = 0;
    private float strafeTimingOffset = 0;
    private int scorePenalties = 0;

    // Reset position values
    public float xReset = 0.0f;  // X-axis reset position
    public float yReset = 1.0f;  // Y-axis reset position
    public float zReset = 0.0f;  // Z-axis reset position



     private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        currentJumpAttempt = new JumpAttempt(1,attemptNumber, 0, 0, 0, 0, 0, 0, 0, 0, 0, date: System.DateTime.Now);
        uiManager.DisableHudElements();
        uiManager.DisableStatScreen();
        uiManager.EnableStartElements();
        playerManager.ResetPlayer(xReset, yReset, zReset);
        maxSwitches = maxJumps;
        
        resetArrays();
    }

    public void startAttempt()
    {
        resetArrays();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        timelineController.RemoveAllPips();
        ///text appears: move mouse to either the left or the right to start an attempt, then switch targets by smoothly moving your mouse to the other target each time the 
        /// indicator changes color.
        /// HUD
        uiManager.EnableHudElements();
        uiManager.DisableStatScreen();
        uiManager.DisableStartElements();
        //Player control enabled
        surfCharacter.movementEnabled = true;
        playerManager.EnableMouseLook();
        //make targets virtual
        lookDirection = true;
        mouseAngleTracker.resetAngleChange();
        mouseAngleTracker.isAttemptActive = true;
        arrow.transform.rotation = Quaternion.Euler(0, 180, 0);
        firstJump = true;
        speedTracker.isAttemptActive = true;
        bhopAccuracy = 0;
        lookOffset = 0;
        scorePenalties = 0;
        switchCount = 1;
        currentJumps = 0;
    }

    public void endAttempt()
    {
        playerManager.DisableMouseLook();
        // Save the score, values are placeholders for now
        attemptNumber++;
        // Update the lastJumpAttempt to the currentJumpAttempt
        // Reset the currentJumpAttempt
        bhopAccuracy = calculateBhopAccuracy();
        float score = (0.65f - Math.Abs(lookOffset))*5f + (0.65f - Math.Abs(strafeTimingOffset))*5f + (1-bhopAccuracy)*10 - scorePenalties;
        currentJumpAttempt = new JumpAttempt(4,attemptNumber, strafeTimingOffset, 0, 0, 0, 0, score, 0, lookOffset, bhopAccuracy, date: System.DateTime.Now);
        scoreManager.SaveScore(currentJumpAttempt);
        //TODO: stats are going to be different depending on the scene, this should probably be dont in the scene manager but I dont know
        //jank, fix later
        uiManager.StatScreen.GetComponent<StrafingStatScreen>().currentJumpAttempt = currentJumpAttempt;
        uiManager.StatScreen.GetComponent<StrafingStatScreen>().lastJumpAttempt = lastJumpAttempt;
        uiManager.StatScreen.GetComponent<StrafingStatScreen>().updateStats();
        timelineController.CalculateTimestamps();
        //reset vars
        lastJumpAttempt = currentJumpAttempt;
        surfCharacter.moveData.velocity = Vector3.zero;
        surfCharacter.moveData.wishJump = false;
        switchTimes.Clear();
        jumpIndicator.deleteDots();
        playerStart = false;
        mouseAngleTracker.isAttemptActive = false;
        //resetArrays(); 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        
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
        jumpTimestamps = Enumerable.Repeat(-1000f, maxJumps).ToArray();
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
                rightSwitchCount = 0;
                leftSwitchCount = -1;
                ATimer = 0;
                DTimer = 0;
                globalTimer = 0;
                switchTimer = 0;
                //You should look right first
                lookDirection = true;
                playerStart = true;
                if(firstJump == true)
                {
                    firstJump = false;
                }
                else
                {
                    arrow.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                jumpIndicator.StartJump();
                rightLookTimes.Add(globalTimer);
            }
            
                    

        }

        /// Jump Tracking
        bool grounded = true;
        //skip first frame, otherwise the player will register an attempt
        if(surfCharacter.groundObject == null)
        {
            grounded = false;
        }

        if (hasJumped == false && grounded == false && playerStart == true)
        {
            // Player has jumped, register the jump
            //print("Jumped");
            hasJumped = true;
            jumpTimestamps[currentJumps] = globalTimer;
            jumpIndicator.StartJump();
        }

        if (hasJumped == true && grounded == true && groundTimer == -1 && playerStart == true)
        {
            // Player landed after a jump, start timing the grounded period
            groundTimer = 0;
            hasJumped = false; // Reset jump status after landing
        }

        if (hasJumped == false && grounded == true && groundTimer >= 0 && playerStart == true)
        {
            // Player is on the ground, increment ground time
            groundTimer += Time.deltaTime;
        }

        if (hasJumped == true && grounded == false && groundTimer >= 0 && playerStart == true)
        {
            // Player has jumped, add the ground time to the list
            groundTimes.Add(groundTimer);
            //Debug.Log("Ground time recorded: " + groundTimer);
            currentJumps++;
            //Debug.Log("Current jumps: " + currentJumps);
            groundTimer = -1; // Reset timer after recording
            //tell player to switch directions
            arrow.transform.Rotate(0, 180, 0);
        }

        // if (currentJumps >= maxJumps)
        // {
        //     // Player has reached max jumps, end the attempt
        //     foreach (float time in groundTimes)
        //     {
        //         //Debug.Log(time);
        //     }
        //     endAttempt();
        //     resetScene();
        // }


        //if attempt is active, add time
        if(playerStart == true)
        {
            globalTimer += Time.deltaTime;
            switchTimer += Time.deltaTime;
            //Strafe logic
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
        //LOOK LOGIC//
        //check if the player has started the attempt, if so, are they looking left or right?
        if(playerStart == true)
        {
            bool newDirection = false;
            if(switchTrackTimer < 0.05)
            {
                switchTrackTimer += Time.deltaTime;
            }
            else
            {
                switchTrackEnd = mouseAngleTracker.angleChange;
                //left is false, true is right
                if(lookDirection == false)
                {
                    if( switchTrackEnd  > switchTrackStart + 3)
                    {
                        Debug.Log("going right");
                        lookDirection = true;
                        newDirection = true;
                    }
                }
                else if(lookDirection == true)
                {
                    if( switchTrackEnd  < switchTrackStart - 3)
                    {
                        Debug.Log("going left");
                        lookDirection = false;
                        newDirection = true;
                    }
                }
                switchTrackTimer = 0;
                switchTrackStart = switchTrackEnd;
            }
            if(newDirection && lookDirection == false && switchCount == currentJumps) //you just switched from right to left
            {
                //TODO: I could track angle change here, new stat?
                switchTimes.Add(switchTimer);
                switchTimer = 0;
                leftLookTimes.Add(globalTimer);
                leftSwitchCount++;
                switchCount++;
                //Debug.Log("Player is looking right");

            }
            else if(newDirection && lookDirection == true && switchCount == currentJumps) //you just switched from left to right
            {
                switchTimes.Add(switchTimer);
                switchTimer = 0;
                rightLookTimes.Add(globalTimer);
                rightSwitchCount++;
                switchCount++;
            }
        }

        //TODO - need to update this to have checks for starting level
        if (Input.GetMouseButtonDown(0) && firstTime == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            firstTime = false;
            startAttempt();
        }

        if ((switchTimes.Count >= maxSwitches || currentJumps >= maxJumps) && playerStart == true)
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
            //calculate strafe accuracy

           //calculate offset of all D presses from switch
            // for(int i = 0; i < rightLookTimes.Count-1; i++)
            // {
            //     if(DPressedTimestamps[i] < rightLookTimes[i] && DPressedTimestamps[i] > rightLookTimes[i] - 0.2f)
            //     {
            //         DPressedOffset[i] = rightLookTimes[i] - DPressedTimestamps[i];
            //     }
            //     else if(DPressedTimestamps[i] > rightLookTimes[i] && DPressedTimestamps[i] < rightLookTimes[i] + 0.2f)
            //     {
            //         DPressedOffset[i] = DPressedTimestamps[i] - rightLookTimes[i];
            //     }               
            // }
            for(int i = 0; i < rightLookTimes.Count-1; i++)
            {
                for(int j = 0; j < DPressedTimestamps.Count(); j++)
                {
                    if(DPressedTimestamps[j] < rightLookTimes[i] && DPressedTimestamps[j] > rightLookTimes[i] - 0.2f)
                    {
                        DPressedOffset[i] = rightLookTimes[i] - DPressedTimestamps[j];
                    }
                    else if(DPressedTimestamps[j] > rightLookTimes[i] && DPressedTimestamps[j] < rightLookTimes[i] + 0.2f)
                    {
                        DPressedOffset[i] = DPressedTimestamps[j] - rightLookTimes[i];
                    }   
                }
            }
            //calculate offset of all D releases from switch
            // for(int i = 0; i < rightLookTimes.Count-1; i++)
            // {
            //     float release = (DPressedOffset[i] + rightLookTimes[i]) + DPressedTimes[i];
            //     if(release < leftLookTimes[i] && release > leftLookTimes[i] - 0.2f)
            //     {
            //         DReleasedOffset[i] = leftLookTimes[i] - release;
            //     }
            //     else if(release > leftLookTimes[i] && release < leftLookTimes[i] + 0.2f)
            //     {
            //         DReleasedOffset[i] = release - leftLookTimes[i];
            //     }
            // }
            for(int i = 0; i < rightLookTimes.Count-1; i++)
            {
                for(int j = 0; j < DPressedTimestamps.Count(); j++)
                {
                    float release = (DPressedOffset[i] + rightLookTimes[i]) + DPressedTimes[j];
                    if(release < leftLookTimes[i] && release > leftLookTimes[i] - 0.2f)
                    {
                        DReleasedOffset[i] = leftLookTimes[i] - release;
                    }
                    else if(release > leftLookTimes[i] && release < leftLookTimes[i] + 0.2f)
                    {
                        DReleasedOffset[i] = release - leftLookTimes[i];
                    }
                }
            }

            //calculate offset of all A presses from switch
            // for(int i = 0; i < leftLookTimes.Count; i++)
            // {
            //     if(APressedTimestamps[i] < leftLookTimes[i] && APressedTimestamps[i] > leftLookTimes[i] - 0.2f)
            //     {
            //         APressedOffset[i] = leftLookTimes[i] - APressedTimestamps[i];
            //     }
            //     else if(APressedTimestamps[i] > leftLookTimes[i] && APressedTimestamps[i] < leftLookTimes[i] + 0.2f)
            //     {
            //         APressedOffset[i] = APressedTimestamps[i] - leftLookTimes[i];
            //     }
            // }
            for(int i = 0; i < leftLookTimes.Count; i++)
            {
                for(int j = 0; j < APressedTimestamps.Count(); j++)
                {
                    if(APressedTimestamps[j] < leftLookTimes[i] && APressedTimestamps[j] > leftLookTimes[i] - 0.2f)
                    {
                        APressedOffset[i] = leftLookTimes[i] - APressedTimestamps[j];
                    }
                    else if(APressedTimestamps[j] > leftLookTimes[i] && APressedTimestamps[j] < leftLookTimes[i] + 0.2f)
                    {
                        APressedOffset[i] = APressedTimestamps[j] - leftLookTimes[i];
                    }
                }
            }
            //calculate offset of all A releases from switch
            // for(int i = 0; i < leftLookTimes.Count; i++)
            // {
            //     float release = (APressedOffset[i] + leftLookTimes[i]) + APressedTimes[i];
            //     if(release < rightLookTimes[i] && release > rightLookTimes[i] - 0.2f)
            //     {
            //         AReleasedOffset[i] = rightLookTimes[i] - release;
            //     }
            //     else if(release > rightLookTimes[i] && release < rightLookTimes[i] + 0.2f)
            //     {
            //         AReleasedOffset[i] = release - rightLookTimes[i] ;
            //     }
            // }
            for(int i = 0; i < leftLookTimes.Count; i++)
            {
                for(int j = 0; j < APressedTimestamps.Count(); j++)
                {
                    float release = (APressedOffset[i] + leftLookTimes[i]) + APressedTimes[j];
                    if(release < rightLookTimes[i] && release > rightLookTimes[i] - 0.2f)
                    {
                        AReleasedOffset[i] = rightLookTimes[i] - release;
                    }
                    else if(release > rightLookTimes[i] && release < rightLookTimes[i] + 0.2f)
                    {
                        AReleasedOffset[i] = release - rightLookTimes[i] ;
                    }
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
                lookOffset += time - 0.65f;
            }
            lookOffset = lookOffset / maxSwitches;
            Debug.Log("Look Offset: " + lookOffset);

            timelineController.startLookTimestamps = leftLookTimes.ToArray();
            timelineController.endLookTimestamps = rightLookTimes.ToArray();
            timelineController.strafeStartTimestamps = APressedTimestamps;
            timelineController.strafeEndTimestamps = DPressedTimestamps;
            timelineController.jumpTimestamps = jumpTimestamps;

            endAttempt();
            resetScene();
        }

    }
}

