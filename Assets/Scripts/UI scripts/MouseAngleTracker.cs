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
    private float angleChangePerInterval = 0f;
    private float timer = 0f;
    public float jumpTimer = 0f;
    public float attemptAngleChange = 0;
    private List<float> angleChanges = new List<float>();
    private List<float> angleChangePerIntervals = new List<float>();
    public bool isAttemptActive = false;
    //Smoothness
    private float startAngle;
    private float elapsedTime;
    private bool tracking;
    private List<float> deviations;
    public float targetAngle = 90f;
    public float targetDuration = 0.65f;
    public List<float> smoothnessPerAttempt = new List<float>();

    public void OnEnable()
    {
        float sensitivityMultiplier = SettingsManager.Instance.GetSensitivity();
        mouseSensitivity = (playerAiming.horizontalSensitivity * sensitivityMultiplier) / 2.1f;
    }
    void Start()
    {
        // Capture the initial mouse position
        lastMouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        lastMouseY = (Input.GetAxis("Mouse Y") * playerAiming.verticalSensitivity  * playerAiming.sensitivityMultiplier) / 2.1f;
    }

    void Update()
    {
        // Increment the timer by the time passed since the last frame
        float mouseX = Input.GetAxis("Mouse X")*2.1f;//mouseSensitivity ;
        
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

            float expectedAngle = (elapsedTime / targetDuration) * targetAngle*mouseSensitivity;
            float currentAngle = Math.Abs(startAngle + mouseX);
            float deviation = currentAngle - expectedAngle;
            deviations.Add(deviation);

            // Stop tracking after target duration
            if (elapsedTime >= targetDuration)
            {
                smoothnessPerAttempt.Add(CalculateAverageAttemptAngleSmoothness());
            }
        }
    }
    public void resetAngleChange()
    {
        angleChange = 0;
    }

    void CalculateMouseAngleSmoothness()
    {

        angleChangePerInterval = accumulatedAngle / timer;

            // Output the angle change for the interval
        //Debug.Log("Angle Change Per Interval: " + angleChangePerInterval);
        if(isAttemptActive)
        {
            //Debug.Log("Angle Change: " + accumulatedAngle);
            angleChanges.Add(accumulatedAngle);
            angleChangePerIntervals.Add(angleChangePerInterval);
        }
            
        // Reset timer and accumulated angle for the next interval
        timer = 0f;
        accumulatedAngle = 0f;

        
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

    public float CalculateAverageAttemptAngleSmoothness()
    {
        tracking = false;

        // Calculate consistency score (lower score = more consistent)
        float totalDeviation = 0f;
        foreach (float deviation in deviations)
        {
            totalDeviation += Mathf.Abs(deviation);
        }
        float consistencyScore = totalDeviation / deviations.Count;
        return consistencyScore;

    }

    public void StartTrackingSmoothness()
    {
        startAngle = Input.GetAxis("Mouse X")*2.1f;
        elapsedTime = 0f;
        deviations = new List<float>();
        tracking = true;
    }
}
