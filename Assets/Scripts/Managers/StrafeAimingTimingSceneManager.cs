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
        resetArrays();
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
        //resetArrays();
        
        
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
        rightLookTimes.Clear();
        leftLookTimes.Clear();
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
            if(Input.GetKeyDown(KeyCode.A)  && APressedTimestamps[leftSwitchCount] == -1000)//&& DHeld == false)
            {
                AHeld = true;
                APressedTimestamps[leftSwitchCount] = (globalTimer);
                Debug.Log("A pressed");
                Debug.Log(globalTimer);
            }
            if(Input.GetKeyDown(KeyCode.D) && DPressedTimestamps[rightSwitchCount] == -1000)//&& AHeld == false)
            {
                DHeld = true;
                DPressedTimestamps[rightSwitchCount] = (globalTimer);
                Debug.Log("D pressed");
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
                Debug.Log("A released");
            }
            if(Input.GetKeyUp(KeyCode.D) && DHeld == true)
            {
                DHeld = false;
                DPressedTimes[rightSwitchCount] = DTimer;
                DTimer = 0;
                Debug.Log("D released");
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
                    leftSwitchCount++;
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
            for(int i = 0; i < leftLookTimes.Count; i++)
            {
                Debug.Log(APressedOffset[i]);
                Debug.Log(AReleasedOffset[i]);
            }

            Debug.Log("D Held Accuracy");
            for(int i = 0; i < rightLookTimes.Count; i++)
            {
                Debug.Log(DPressedOffset[i]);
                Debug.Log(DReleasedOffset[i]);
            }


            //for each switch, check from halfway to last switch to halfway to next switch and determine accuracy

            endAttempt();
            resetScene();
        }

    }
}
