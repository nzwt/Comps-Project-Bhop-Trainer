using UnityEngine;
using TMPro;
using Fragsurf.Movement;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

public class MouseAngleTracker : MonoBehaviour
{
    public TextMeshProUGUI angleText;   // TextMeshProUGUI to display angle change
    public float updateInterval = 1f; // Update interval (0.1 seconds)
    public float mouseSensitivity = 1f;  // Sensitivity multiplier to convert input to degrees  
    public PlayerAiming playerAiming; // Reference to the PlayerAiming script
    private float lastMouseX;
    private float lastMouseY;
    private float accumulatedAngle = 0f;
    public float angleChange = 0f;
    private float timer = 0f;
    public float jumpTimer = 0f;
    public float attemptAngleChange = 0;
    private List<float> angleChanges = new List<float>();
    private List<float> angleChangePerIntervals = new List<float>();
    public bool isAttemptActive = false;
    //Smoothness
    private float startAngle;
    private float elapsedTime;
    private float smoothnessTimer;
    private float smoothnessStartAngle;
    private bool tracking;
    public List<float> deviations;
    public float targetAngle = 90f;
    public float targetDuration = 0.65f;
    public List<float> smoothnessPerAttempt = new List<float>();
    public bool movePositive = true;
    int changeCounter = 1;    public bool isPaused = false;


    public void OnEnable()
    {
        // float sensitivityMultiplier = 2.1f;
        // if(SettingsManager.Instance != null)
        // {
        //     sensitivityMultiplier = SettingsManager.Instance.GetSensitivity();
        // }
        mouseSensitivity = (playerAiming.horizontalSensitivity * SettingsManager.Instance.GetSensitivity());
    }
    void Start()
    {
        // Capture the initial mouse position
        lastMouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        smoothnessStartAngle = lastMouseX;
        lastMouseY = (Input.GetAxis("Mouse Y") * playerAiming.verticalSensitivity  * playerAiming.sensitivityMultiplier) / 2.1f;
    }

    void Update()
    {
        if(isPaused)
        {
            return;
        }
        // Increment the timer by the time passed since the last frame   
        float mouseX = Input.GetAxis("Mouse X")*mouseSensitivity;
        
        // Accumulate the angle change
        accumulatedAngle += mouseX;
        angleChange += mouseX;
        angleChange = angleChange%360;
        
        // Track time
        timer += Time.deltaTime;

        //for recording the angle change of an attempt
        if (!isAttemptActive && angleChanges.Count > 0) // When attempt is finished, calculate average.
        {
            float attemptAngleChange = CalculateAttemptAngleChange();
            Debug.Log("Average Angle Change: " + attemptAngleChange);
            angleChanges = new List<float>(); // Reset for the next attempt.
            angleChange = 0;
        }

        //Smoothness:
        if (tracking)
        {
            elapsedTime += Time.deltaTime;
            smoothnessTimer += Time.deltaTime;

            if(smoothnessTimer >= 0.05)
            {
                float expectedAngle = ((targetAngle/13) * changeCounter);
                float currentAngle = 0;
                if( movePositive)
                {
                    expectedAngle = smoothnessStartAngle + expectedAngle;
                    currentAngle = angleChange;
                }
                else
                {
                    expectedAngle = smoothnessStartAngle - expectedAngle;
                    currentAngle = angleChange;
                }
                float deviation = Mathf.Abs(currentAngle - expectedAngle);
                print("Deviation: " + deviation);
                print("Current Angle: " + currentAngle);
                print("Expected Angle: " + expectedAngle);
                deviations.Add(deviation);
                smoothnessTimer = 0;
                changeCounter++;
            }

            // Stop tracking after target duration
            if (elapsedTime >= targetDuration)
            {
                smoothnessTimer = 0;
                smoothnessPerAttempt.Add(CalculateIntervalSmoothness());
            }
        }
    }
    public void resetAngleChange()
    {
        angleChange = 0;
    }

    public float CalculateAttemptAngleChange()
    {
        float totalChange = 0;
        foreach (float change in angleChanges)
        {
            totalChange += change;
        }
        return totalChange;
    }

    public float CalculateIntervalSmoothness()
    {
        tracking = false;

        // Calculate consistency score (lower = more consistent)
        float totalDeviation = 0f;
        foreach (float deviation in deviations)
        {
            totalDeviation += Mathf.Abs(deviation);
        }
        float consistencyScore = totalDeviation / deviations.Count;
        return consistencyScore;

    }

    public float CalculateAverageAttemptAngleSmoothness()
    {
        tracking = false;
        float averageSmoothness = 0f;
        foreach (float smoothness in smoothnessPerAttempt)
        {
            averageSmoothness += smoothness;
        }
        return averageSmoothness / smoothnessPerAttempt.Count;

    }

    public void StartTrackingSmoothness()
    {
        //TODO replace with sensitivity
        startAngle = Input.GetAxis("Mouse X")*mouseSensitivity;
        elapsedTime = 0f;
        deviations = new List<float>();
        tracking = true;
        changeCounter = 1;
        smoothnessStartAngle = angleChange;
    }
}
