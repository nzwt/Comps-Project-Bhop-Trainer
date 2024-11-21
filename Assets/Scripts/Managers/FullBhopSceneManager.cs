using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fragsurf.Movement;
using System;
using System.Linq;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class FullBhopSceneManager : MonoBehaviour
{
 public SurfCharacter surfCharacter;
    public UIManager uiManager;
    public PlayerManager playerManager;
    public ScoreManager scoreManager;
    public ArcJumpIndicator jumpIndicator;
    public MouseAngleTracker mouseAngleTracker;
    public SpeedTracker speedTracker;
    public ZAxisSpeedTracker zAxisSpeedTracker;
    public TimelineController timelineController;
    // game objects
    public JumpAttempt currentJumpAttempt;
    public JumpAttempt lastJumpAttempt;
    public int attemptNumber = 0;
    public GameObject cameraHolder;
    public GameObject arrow; //flip x to aim left
    //Race Values
    public bool isRace = false;
    public float finishLine = 15;
    private float raceTimer = 0;
    
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
    //these are for the closest look attempts to the jump
    public float[] rightLookAttemptTimestamps;
    public float[] rightLookOffsets;
    public float[] leftLookAttemptTimestamps;
    public float[] leftLookOffsets;
    public float[] jumpTimestamps;
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
    public float[] AStrafeTimeline;
    public float[] DStrafeTimeline;
    public float[] AStrafeTimelineEnd;
    public float[] DStrafeTimelineEnd;
    private float ATimer = -1;
    private float DTimer = -1;
    private float switchTimer = -1;
    private float globalTimer = -1;
    [SerializeField]
    private int rightSwitchCount = -1;
    [SerializeField]
    private int leftSwitchCount = -1;
    //false is left, true is right
    private int AHeldCount = 0;
    private int DHeldCount = 0;
    public bool lookDirection = false;

    // Jumping Variables
    public bool hasJumped = false;
    public bool firstJump = true;
    public bool allowPlayerMovement = true;
    // jump management values
    [SerializeField]
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
        if(isRace)
        {
            scoreManager.LoadScores(4);
        }
        else
        {
            scoreManager.LoadScores(3);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        currentJumpAttempt = new JumpAttempt(3,attemptNumber, 0, 0, 0, 0, 0, 0, 0, 0, 0, date: System.DateTime.Now);
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
        arrow.transform.rotation = Quaternion.Euler(0, 0, 0);
        firstJump = true;
        speedTracker.isAttemptActive = true;
        bhopAccuracy = 0;
        lookOffset = 0;
        scorePenalties = 0;
        switchCount = 1;
        currentJumps = 0;
        if(isRace)
        {
            raceTimer = 0;
        }
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
        currentJumpAttempt = new JumpAttempt(4,attemptNumber, strafeTimingOffset, raceTimer, 0, 0, zAxisSpeedTracker.calculateAttemptSpeed(), score, 0, lookOffset, bhopAccuracy, date: System.DateTime.Now);
        if(isRace)
        {
            scoreManager.SaveScore(4, currentJumpAttempt);
        }
        else
        {
            scoreManager.SaveScore(3, currentJumpAttempt);
        }
        //TODO: stats are going to be different depending on the scene, this should probably be dont in the scene manager but I dont know
        //jank, fix later
        if(!isRace)
        {
            uiManager.StatScreen.GetComponent<FullBhopStatScreen>().currentJumpAttempt = currentJumpAttempt;
            uiManager.StatScreen.GetComponent<FullBhopStatScreen>().lastJumpAttempt = lastJumpAttempt;
            uiManager.StatScreen.GetComponent<FullBhopStatScreen>().updateStats();
        }
        else
        {
            uiManager.StatScreen.GetComponent<RaceBhopStatScreen>().currentJumpAttempt = currentJumpAttempt;
            uiManager.StatScreen.GetComponent<RaceBhopStatScreen>().lastJumpAttempt = lastJumpAttempt;
            uiManager.StatScreen.GetComponent<RaceBhopStatScreen>().updateStats();
        }
        timelineController.CalculateTimestamps();
        //reset vars
        lastJumpAttempt = currentJumpAttempt;
        surfCharacter.moveData.velocity = Vector3.zero;
        surfCharacter.moveData.wishJump = false;
        surfCharacter.wishJumpScroll = 0;
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
        int rightJumps = maxJumps/2;
        if(maxJumps%2 == 1)
        {
            rightJumps = (maxJumps/2)+1;
        }
        APressedTimestamps = Enumerable.Repeat(-1000f, 1000).ToArray();
        DPressedTimestamps = Enumerable.Repeat(-1000f, 1000).ToArray();
        APressedTimes = Enumerable.Repeat(-1000f, 1000).ToArray();
        DPressedTimes = Enumerable.Repeat(-1000f, 1000).ToArray();
        APressedOffset = Enumerable.Repeat(-1000f, maxJumps/2).ToArray();
        DPressedOffset = Enumerable.Repeat(-1000f, rightJumps).ToArray();
        AReleasedOffset = Enumerable.Repeat(-1000f, maxJumps/2).ToArray();
        DReleasedOffset = Enumerable.Repeat(-1000f, rightJumps).ToArray();
        AStrafeTimeline = Enumerable.Repeat(-1000f, maxSwitches/2).ToArray();
        DStrafeTimeline = Enumerable.Repeat(-1000f, rightJumps).ToArray();
        AStrafeTimelineEnd = Enumerable.Repeat(-1000f, maxJumps/2).ToArray();
        DStrafeTimelineEnd = Enumerable.Repeat(-1000f, rightJumps).ToArray();
        rightLookTimes = new List<float>();
        leftLookTimes = new List<float>();
        rightLookAttemptTimestamps = Enumerable.Repeat(-1000f, rightJumps).ToArray();
        leftLookAttemptTimestamps = Enumerable.Repeat(-1000f, maxJumps/2).ToArray();
        rightLookOffsets = Enumerable.Repeat(-1000f, rightJumps).ToArray();
        leftLookOffsets = Enumerable.Repeat(-1000f, maxJumps/2).ToArray();
        jumpTimestamps = Enumerable.Repeat(-1000f, maxJumps).ToArray();
        groundTimes = new List<float>();
    }

    public void resetScene()
    {
        playerManager.ResetPlayer(xReset, yReset, zReset);
        uiManager.DisableHudElements();
        uiManager.EnableStatScreen();
        uiManager.EnableStartElements();
        startTriggered = false;
    }

        public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
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
                zAxisSpeedTracker.isAttemptActive = true;
                playerStart = true;
                rightSwitchCount = 0;
                leftSwitchCount = -1;
                AHeldCount = 0;
                DHeldCount = 0;
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

        //if attempt is active, add time
        if(playerStart == true)
        {
            globalTimer += Time.deltaTime;
            switchTimer += Time.deltaTime;
            if(isRace)
            {
                raceTimer += Time.deltaTime;
            }
            //Strafe logic
            //TODO: Bug - if the player holds D then lets go and taps A a bunch of times before looking left, it makes the array too big
            if( leftSwitchCount != -1 && Input.GetKeyDown(KeyCode.A)  && APressedTimestamps[AHeldCount] == -1000)//&& DHeld == false)
            {
                AHeld = true;
                APressedTimestamps[AHeldCount] = (globalTimer);
                // Debug.Log("A pressed");
                // Debug.Log(globalTimer);
            }
            if(Input.GetKeyDown(KeyCode.D) && DPressedTimestamps[DHeldCount] == -1000)//&& AHeld == false)
            {
                DHeld = true;
                DPressedTimestamps[DHeldCount] = (globalTimer);
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
                APressedTimes[AHeldCount] = ATimer;
                AHeldCount++;
                ATimer = 0;
                // Debug.Log("A released");
            }
            if(Input.GetKeyUp(KeyCode.D) && DHeld == true)
            {
                DHeld = false;
                DPressedTimes[DHeldCount] = DTimer;
                DHeldCount++;
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
                    if( switchTrackEnd  > switchTrackStart + (3 * 2.1))//SettingsManager.Instance.GetSensitivity()))
                    {
                        Debug.Log("going right");
                        if(!lookDirection)
                        {
                            newDirection = true;
                        }
                        lookDirection = true;
                        
                    }
                }
                else if(lookDirection == true)
                {
                    if( switchTrackEnd  < switchTrackStart - (3 * 2.1))//SettingsManager.Instance.GetSensitivity()))
                    {
                        Debug.Log("going left");
                        if(lookDirection)
                        {
                            newDirection = true;
                        }
                        lookDirection = false;
                    }
                }
                switchTrackTimer = 0;
                switchTrackStart = switchTrackEnd;
            }
            if(newDirection && lookDirection == false) //you just switched from right to left
            {
                //TODO: I could track angle change here, new stat?
                switchTimes.Add(switchTimer);
                switchTimer = 0;
                leftLookTimes.Add(globalTimer);
                leftSwitchCount++;
                switchCount++;
                //Debug.Log("Player is looking right");

            }
            else if(newDirection && lookDirection == true) //you just switched from left to right
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

        if ((currentJumps >= maxJumps) && playerStart == true || (isRace &&(surfCharacter.transform.position.z <= finishLine + 0.2 && surfCharacter.transform.position.z >= finishLine)))
        {
            if(leftSwitchCount == -1)
            {
                leftSwitchCount = 0;
            }
            // Player has reached max switches, end the attempt
            //If the player did not let go of the key, end the time
            if(AHeld == true)
            {
                APressedTimes[AHeldCount] = ATimer;
            }
            else if(DHeld == true)
            {
                DPressedTimes[DHeldCount] = DTimer;
            }
            //calculate look accuracy
            float endTime = globalTimer;
            //calculate strafe accuracy

            //calculate closest look attempts to the jump
            int rightIndex = 0;
            int leftIndex = 0;
            bool right = true;
            for(int i = 0; i < jumpTimestamps.Length; i++)
            {
                float closestRight = float.MaxValue;
                float closestLeft = float.MaxValue;
                print("i: " + i);
                print("rightIndex: " + rightIndex);
                print("leftIndex: " + leftIndex);
                if(right)
                {
                    for(int j = 0; j < rightLookTimes.Count; j++)
                    {
                        if(Math.Abs(jumpTimestamps[i] - rightLookTimes[j]) < closestRight)
                        {
                            closestRight = Math.Abs(jumpTimestamps[i] - rightLookTimes[j]);
                            rightLookAttemptTimestamps[rightIndex] = rightLookTimes[j];
                            rightLookOffsets[rightIndex] = closestRight;
                        }
                    }
                    right = false;
                    rightIndex++;
                }
                else
                {
                    if (leftLookTimes.Count > 0)
                    {
                        for(int j = 0; j < leftLookTimes.Count; j++)
                        {
                            if(Math.Abs(jumpTimestamps[i] - leftLookTimes[j]) < closestLeft)
                            {
                                closestLeft = Math.Abs(jumpTimestamps[i] - leftLookTimes[j]);
                                leftLookAttemptTimestamps[leftIndex] = leftLookTimes[j];
                                leftLookOffsets[leftIndex] = closestLeft;
                            }
                        }
                    }
                    right = true;
                    leftIndex++;
                }
            }

            //calculate offset of all D presses from rightLookAttemptTimestamps
            for(int i = 0; i < rightLookAttemptTimestamps.Count(); i++)
            {
                float closestRight = 1000;
                for(int j = 0; j < DPressedTimestamps.Length; j++)
                {
                    if(Math.Abs(rightLookAttemptTimestamps[i] - DPressedTimestamps[j]) < closestRight)
                    {
                        closestRight = Math.Abs(rightLookAttemptTimestamps[i] - DPressedTimestamps[j]);
                        DPressedOffset[i] = closestRight;
                        DStrafeTimeline[i] = DPressedTimestamps[j];
                        //unsure if I need this, but could be helpful
                        DStrafeTimelineEnd[i] = DPressedTimestamps[j] + DPressedTimes[j];
                    }
                }
            }


            //calculate offset of all A presses from switch
            for(int i = 0; i < leftLookAttemptTimestamps.Count(); i++)
            {
                float closestLeft = 1000;
                for(int j = 0; j < APressedTimestamps.Length; j++)
                {
                    if(Math.Abs(leftLookAttemptTimestamps[i] - APressedTimestamps[j]) < closestLeft)
                    {
                        closestLeft = Math.Abs(leftLookAttemptTimestamps[i] - APressedTimestamps[j]);
                        APressedOffset[i] = closestLeft;
                        AStrafeTimeline[i] = APressedTimestamps[j];
                        AStrafeTimelineEnd[i] = APressedTimestamps[j] + APressedTimes[j];
                    }
                }
            }

            //calculate strafe accuracy
            Debug.Log("A Held Accuracy");
            float totalOffset = 0;
            int totalCounted = 0;
            for(int i = 0; i < leftLookAttemptTimestamps.Count(); i++)
            {
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
            for(int i = 0; i < rightLookAttemptTimestamps.Count(); i++)
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

            int switches = 0;
            //calculate timing offset
            foreach (float offset in rightLookOffsets)
            {
                if(offset != -1000)
                {
                    switches++;
                    lookOffset += offset;
                }
                else
                {
                    scorePenalties += 1;
                }
            }
            foreach (float offset in leftLookOffsets)
            {
                if(offset != -1000)
                {
                    switches++;
                    lookOffset += offset;
                }
                else
                {
                    scorePenalties += 1;
                }
            }
            lookOffset = lookOffset / switches;
            Debug.Log("Look Offset: " + lookOffset);

            timelineController.startLookTimestamps = leftLookTimes.ToArray();
            timelineController.endLookTimestamps = rightLookTimes.ToArray();
            timelineController.strafeStartTimestamps = DStrafeTimeline;
            timelineController.strafeEndTimestamps = AStrafeTimeline;
            timelineController.jumpTimestamps = jumpTimestamps;
            zAxisSpeedTracker.isAttemptActive = false;

            endAttempt();
            resetScene();
        }

    }
}

