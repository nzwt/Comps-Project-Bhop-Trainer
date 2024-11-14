using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fragsurf.Movement;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class StrafeAimingTimingSceneManager : MonoBehaviour
{
 public SurfCharacter surfCharacter;
    public UIManager uiManager;
    public PlayerManager playerManager;
    public ScoreManager scoreManager;
    public ArcJumpIndicator jumpIndicator;
    public MouseAngleTracker mouseAngleTracker;
    public OrbController orbController;
    public TimelineController timelineController;

    // game objects
    public JumpAttempt currentJumpAttempt;
    public JumpAttempt lastJumpAttempt;
    public int attemptNumber = 0;
    public GameObject cameraHolder;
    public GameObject arrow; //flip x to aim left
    
    // managment bools
    public bool firstTime = true;
    public bool lastScoreLoaded = false;
    public bool startTriggered = false;
    public bool playerStart = false;
    private bool AHeld = false;
    private bool DHeld = false;
    //management values
    public bool startPressed = false;
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
    //0 is left, 1 is right
    public Transform currentTarget;
    public Transform leftTarget;
    public Transform rightTarget;
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
        scoreManager.LoadScores(2);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        currentJumpAttempt = new JumpAttempt(1,attemptNumber, 0, 0, 0, 0, 0, 0, 0, 0, 0, date: System.DateTime.Now);
        uiManager.DisableHudElements();
        uiManager.DisableStatScreen();
        uiManager.EnableStartElements();
        playerManager.ResetPlayer(xReset, yReset, zReset);
        currentTarget = leftTarget;
        resetArrays();
    }

    public void startAttempt()
    {
        startPressed = true;
        timelineController.RemoveAllPips();
        ///text appears: move mouse to either the left or the right to start an attempt, then switch targets by smoothly moving your mouse to the other target each time the 
        /// indicator changes color.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        uiManager.EnableHudElements();
        uiManager.DisableStatScreen();
        uiManager.DisableStartElements();
        surfCharacter.movementEnabled = false;
        playerManager.EnableMouseLook();
        currentTarget = leftTarget;
        mouseAngleTracker.resetAngleChange();
        mouseAngleTracker.isAttemptActive = true;
        arrow.transform.rotation = Quaternion.Euler(0, 180, 0);
        orbController.resetTargets();
        bhopAccuracy = 0;
        scorePenalties = 0;
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("Full Bhop Scene");
    }

        public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    public void endAttempt()
    {
        playerManager.DisableMouseLook();
        // Save the score, values are placeholders for now
        attemptNumber++;
        // Update the lastJumpAttempt to the currentJumpAttempt
        // Reset the currentJumpAttempt
        float score = (0.65f - Math.Abs(bhopAccuracy))*7.65f + (0.65f - Math.Abs(strafeTimingOffset))*7.65f;
        currentJumpAttempt = new JumpAttempt(2,attemptNumber, strafeTimingOffset, 0, 0, 0, 0, score, 0, 0, bhopAccuracy, date: System.DateTime.Now);
        scoreManager.SaveScore(2,currentJumpAttempt);
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
        resetArrays();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        startPressed = false;
        
        
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
        //start attempt by moving over left orb
        if(playerStart == false && startPressed == true)
        {
            if(mouseAngleTracker.angleChange < 0)
            {
                if (mouseAngleTracker.angleChange < -40 && mouseAngleTracker.angleChange > -45)
                {
                    playerStart = true;
                    //Debug.Log("Player is looking left");
                    switchTimer = 0;
                    globalTimer = 0;
                    rightSwitchCount = 0;
                    leftSwitchCount = -1;
                    ATimer = 0;
                    DTimer = 0;
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
                if(mouseAngleTracker.angleChange > 40 && mouseAngleTracker.angleChange < 45 && currentTarget == rightTarget && switchTimer > -1)
                {
                    switchTimes.Add(switchTimer);
                    orbController.ShowRightAccuracy(switchTimer);
                    switchTimer = 0;
                    arrow.transform.Rotate(0, 180, 0);
                    currentTarget = leftTarget;
                    jumpIndicator.StartJump();
                    leftLookTimes.Add(globalTimer);
                    leftSwitchCount++;
                    //Debug.Log("Player is looking right");
                }
            }
            else if(mouseAngleTracker.angleChange < 0)
            {
                if (mouseAngleTracker.angleChange < -40 && mouseAngleTracker.angleChange > -45 && currentTarget == leftTarget && switchTimer > -1)
                {
                    switchTimes.Add(switchTimer);
                    orbController.ShowLeftAccuracy(switchTimer);
                    switchTimer = 0;
                    arrow.transform.Rotate(0, 180, 0);
                    currentTarget = rightTarget;
                    jumpIndicator.StartJump();
                    rightLookTimes.Add(globalTimer);
                    rightSwitchCount++;
                    //Debug.Log("Player is looking left");
                }
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
                bhopAccuracy += time - 0.65f;
            }
            bhopAccuracy = bhopAccuracy / maxSwitches;
            //for each switch, check from halfway to last switch to halfway to next switch and determine accuracy
            //add values to timeline
            timelineController.startLookTimestamps = leftLookTimes.ToArray();
            timelineController.endLookTimestamps = rightLookTimes.ToArray();
            timelineController.strafeStartTimestamps = APressedTimestamps;
            timelineController.strafeEndTimestamps = DPressedTimestamps;

            endAttempt();
            resetScene();
        }

    }
}
