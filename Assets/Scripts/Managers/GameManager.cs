using System.Collections;
using System.Collections.Generic;
using Fragsurf.Movement;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Hud Elements
    [SerializeField]
    public GameObject[] HudElements = new GameObject[3];
    [SerializeField]
    public GameObject[] StartElements = new GameObject[3];
    [SerializeField]
    public GameObject StatScreen;
    [SerializeField]
    // Reset position values
    private float xReset = 0.0f;  // X-axis reset position
    [SerializeField]
    private float yReset = 1.0f;  // Y-axis reset position
    [SerializeField]
    private float zReset = 0.0f;  // Z-axis reset position
    [SerializeField]
    // Reference to the SurfCharacter script
    private SurfCharacter surfCharacter;
    [SerializeField]
    private PlayerAiming playerAiming;
    public ScoreManager scoreManager;
    
    public bool hasJumped = false;
    public int attemptNumber = 0;
    public bool firstFrame = true;
    //stats
    public JumpAttempt lastJumpAttempt;
    public JumpAttempt currentJumpAttempt;
    public MouseAngleTracker mouseAngleTracker;
    public SpeedTracker speedTracker;


    void ResetPlayer()
    {
        // Reset the player's position and rotation
        surfCharacter.transform.position = new Vector3(xReset, yReset, zReset);
        surfCharacter.transform.rotation = Quaternion.LookRotation(Vector3.forward);
        //disable input
        surfCharacter.moveData.verticalAxis = 0;
        surfCharacter.moveData.horizontalAxis = 0;
        surfCharacter.movementEnabled = false;
        mouseAngleTracker.isAttemptActive = false;
        speedTracker.isAttemptActive = false;
        DisableMouseLook();
    }

    private void OnEnable()
    {
        currentJumpAttempt = new JumpAttempt(attemptNumber, 0, 0, 0, 0, 0, 0, 0, 0, date: System.DateTime.Now);
        DisableHudElements();
        DisableStatScreen();
        EnableStartElements();
        ResetPlayer();
    }

    private void OnDisable()
    {
    }

    public void EnableHudElements()
    {
        for (int i = 0; i < HudElements.Length; i++)
        {
            HudElements[i].SetActive(true);
        }
    }

    public void DisableHudElements()
    {
        for (int i = 0; i < HudElements.Length; i++)
        {
            HudElements[i].SetActive(false);
        }
    }
    
    public void EnableStartElements()
    {
        for (int i = 0; i < StartElements.Length; i++)
        {
            StartElements[i].SetActive(true);
        }
    }

    public void DisableStartElements()
    {
        for (int i = 0; i < StartElements.Length; i++)
        {
            StartElements[i].SetActive(false);
        }
    }
    
    public void EnableStatScreen()
    {
        StatScreen.SetActive(true);
    }
    public void DisableStatScreen()
    {
        StatScreen.SetActive(false);
    }
    
    public void DisableMouseLook()
    {
        playerAiming.resetRotation();
        playerAiming.canAim = false;
    }

    public void EnableMouseLook()
    {
        playerAiming.canAim = true;
    }

    public void resetScene()
    {
        ResetPlayer();
        DisableHudElements();
        EnableStatScreen();
        EnableStartElements();
    }

    public void endAttempt()
    {
        // Save the score, values are placeholders for now
        attemptNumber++;
        // Update the lastJumpAttempt to the currentJumpAttempt
        // Reset the currentJumpAttempt
        lastJumpAttempt = currentJumpAttempt;
        currentJumpAttempt = new JumpAttempt(attemptNumber, 0, 0, 0, 0, speedTracker.currentAttemptSpeed, 0, mouseAngleTracker.CalculateAttemptAngleChange(), 0, date: System.DateTime.Now);
        scoreManager.SaveScore(currentJumpAttempt);
        StatScreen.GetComponent<StatScreen>().updateStats();
        hasJumped = false;
        mouseAngleTracker.isAttemptActive = false;
        speedTracker.isAttemptActive = false;
        
        
    }

    
    // Start is called before the first frame update
    void Start()
    {
        scoreManager.LoadScores();
        //load the most recent score
        if (scoreManager.scoreList.Count > 0)
            {
                // Access the last element in the scoreList and return its attempt number
                lastJumpAttempt = scoreManager.scoreList[scoreManager.scoreList.Count - 1];
                attemptNumber = scoreManager.scoreList[scoreManager.scoreList.Count - 1].attemptNumber;
            }
    }

    // Update is called once per frame
    void Update()
    {   
        //skip first frame, otherwise the player will register an attempt
        bool grounded = surfCharacter.moveData.groundedTemp;
        if(firstFrame)
        {
            grounded = true;
            firstFrame = false;
        }
        //TODO - need to update this to have checks for starting level
        if (Input.GetMouseButtonDown(0))
        {
            EnableHudElements();
            DisableStatScreen();
            DisableStartElements();
            surfCharacter.movementEnabled = true;
            EnableMouseLook();
            mouseAngleTracker.isAttemptActive = true;
            speedTracker.isAttemptActive = true;
        }
        if( hasJumped == false && grounded == false)
        {
            print("Jumped");
            hasJumped = true;
        }
        if (hasJumped == true && grounded == true)
        {
            endAttempt();
            resetScene();
        }
    }
}
