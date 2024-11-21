using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fragsurf.Movement;
using System;
using UnityEngine.SceneManagement;

public class AimingTimingSceneManager : MonoBehaviour
{
    public SurfCharacter surfCharacter;
    public UIManager uiManager;
    public PlayerManager playerManager;
    public ScoreManager scoreManager;
    public ArcJumpIndicator jumpIndicator;
    public MouseAngleTracker mouseAngleTracker;
    public OrbController orbController;

    // game objects
    public JumpAttempt currentJumpAttempt;
    public JumpAttempt lastJumpAttempt;
    public int attemptNumber = 0;
    public GameObject cameraHolder;
    public GameObject arrow; //flip x to aim left
    
    // managment bools
    public bool startPressed = false;
    public bool lastScoreLoaded = false;
    public bool startTriggered = false;
    public bool playerStart = false;
    public bool firstTime = true;
    //management values
    public int maxSwitches = 5;
    public List<float> switchTimes = new List<float>();
    private float switchTimer = -1;
    private float globalTimer = -1;
    private float sensitivity;
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
        scoreManager.LoadScores(1);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        currentJumpAttempt = new JumpAttempt(1,attemptNumber, 0, 0, 0, 0, 0, 0, 0, 0, 0, date: System.DateTime.Now);
        uiManager.DisableHudElements();
        uiManager.DisableStatScreen();
        uiManager.EnableStartElements();
        playerManager.ResetPlayer(xReset, yReset, zReset);
        currentTarget = leftTarget;
        sensitivity = 2.1f;//SettingsManager.Instance.GetSensitivity();
    }

    public void startAttempt()
    {
        startPressed = true;
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
        mouseAngleTracker.smoothnessPerAttempt = new List<float>();
        arrow.transform.rotation = Quaternion.Euler(0, 0, 0);
        orbController.resetTargets();
        bhopAccuracy = 0;
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("Strafe Aiming timing scene");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void endAttempt()
    {
        playerManager.DisableMouseLook();
        // Save the score, values are placeholders for now
        attemptNumber++;
        // Update the lastJumpAttempt to the currentJumpAttempt
        // Reset the currentJumpAttempt]
        float smoothness = mouseAngleTracker.CalculateAverageAttemptAngleSmoothness();
        float score = (0.65f - Math.Abs(bhopAccuracy))*15.4f;
        for(int i = 0; i < mouseAngleTracker.smoothnessPerAttempt.Count; i++)
        {
            Debug.Log(mouseAngleTracker.smoothnessPerAttempt[i]);
        }
        currentJumpAttempt = new JumpAttempt(1,attemptNumber, 0, 0, smoothness, 0, 0, score, 0, 0, bhopAccuracy, date: System.DateTime.Now);
        scoreManager.SaveScore(1,currentJumpAttempt);
        //TODO: stats are going to be different depending on the scene, this should probably be dont in the scene manager but I dont know
        //jank, fix later
        uiManager.StatScreen.GetComponent<AimingStatScreen>().currentJumpAttempt = currentJumpAttempt;
        uiManager.StatScreen.GetComponent<AimingStatScreen>().lastJumpAttempt = lastJumpAttempt;
        uiManager.StatScreen.GetComponent<AimingStatScreen>().updateStats();
        //reset vars
        lastJumpAttempt = currentJumpAttempt;
        surfCharacter.moveData.velocity = Vector3.zero;
        surfCharacter.moveData.wishJump = false;
        switchTimes.Clear();
        jumpIndicator.deleteDots();
        playerStart = false;
        mouseAngleTracker.isAttemptActive = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        startPressed = false;
        //mouseAngleTracker.smoothnessPerAttempt.Clear();
    }



    public void resetScene()
    {
        playerManager.ResetPlayer(xReset, yReset, zReset);
        uiManager.DisableHudElements();
        uiManager.EnableStatScreen();
        uiManager.EnableStartElements();
        startTriggered = false;
    }
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
                if (mouseAngleTracker.angleChange < -87.5 && mouseAngleTracker.angleChange > -92.5)
                {
                    playerStart = true;
                    //Debug.Log("Player is looking left");
                    switchTimer = 0;
                    currentTarget = rightTarget;
                    orbController.TargetLeft();
                    arrow.transform.rotation = Quaternion.Euler(0, 0, 0);
                    jumpIndicator.StartJump();
                    mouseAngleTracker.StartTrackingSmoothness();
                    mouseAngleTracker.movePositive = true;
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
                if(mouseAngleTracker.angleChange > 87.5 && mouseAngleTracker.angleChange < 92.5 && currentTarget == rightTarget && switchTimer > -1)
                {
                    switchTimes.Add(switchTimer);
                    orbController.ShowRightAccuracy(switchTimer);
                    switchTimer = 0;
                    arrow.transform.Rotate(0, 180, 0);
                    currentTarget = leftTarget;
                    jumpIndicator.StartJump();
                    //Debug.Log("Player is looking right");
                    mouseAngleTracker.StartTrackingSmoothness();
                    mouseAngleTracker.movePositive = false;
                }
            }
            else if(mouseAngleTracker.angleChange < 0)
            {
                if (mouseAngleTracker.angleChange < -87.5 && mouseAngleTracker.angleChange > -92.5 && currentTarget == leftTarget && switchTimer > -1)
                {
                    switchTimes.Add(switchTimer);
                    orbController.ShowLeftAccuracy(switchTimer);
                    switchTimer = 0;
                    arrow.transform.Rotate(0, 180, 0);
                    currentTarget = rightTarget;
                    jumpIndicator.StartJump();
                    mouseAngleTracker.StartTrackingSmoothness();
                    mouseAngleTracker.movePositive = true;
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
            foreach (float time in switchTimes)
            {
                bhopAccuracy += time - 0.65f;
            }
            //this stat is actually look offset accuracy
            bhopAccuracy = bhopAccuracy / maxSwitches;
            endAttempt();
            resetScene();
        }

    }
}