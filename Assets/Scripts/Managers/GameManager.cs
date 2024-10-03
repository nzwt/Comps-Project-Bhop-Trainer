using System;
using System.Collections;
using System.Collections.Generic;
using Fragsurf.Movement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Hud Elements
    [SerializeField]
    public GameObject HudElements;
    [SerializeField]
    public GameObject StartElements;
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
    public bool lastScoreLoaded = false;
    public bool startTriggered = false;
    //stats
    public JumpAttempt lastJumpAttempt = null;
    public JumpAttempt currentJumpAttempt;
    public MouseAngleTracker mouseAngleTracker;
    public SpeedTracker speedTracker;
    //scene bool
    //not sure if this is how I want to do this
    public bool allowPlayerMovement = true;


    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    //TODO get rid of these functions, not needed anymore, just toggle on and off individually

    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {   

    }
}
