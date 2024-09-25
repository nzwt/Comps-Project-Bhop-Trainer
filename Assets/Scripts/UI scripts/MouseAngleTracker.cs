using UnityEngine;
using TMPro;
using Fragsurf.Movement;
using System.Collections.Generic;

public class MouseAngleTracker : MonoBehaviour
{
    public TextMeshProUGUI angleText;   // TextMeshProUGUI to display angle change
    public float updateInterval = 1f; // Update interval (0.1 seconds)
    public float mouseSensitivity = 1f;  // Sensitivity multiplier to convert input to degrees  
    public PlayerAiming playerAiming; // Reference to the PlayerAiming script
    private float lastMouseX;
    private float lastMouseY;
    private float accumulatedAngle = 0f;
    private float angleChangePerInterval = 0f;
    private float timer = 0f;
    public float attemptAngleChange = 0;
    private List<float> angleChanges = new List<float>();
    public bool isAttemptActive = false;

    void Start()
    {
        mouseSensitivity = (playerAiming.horizontalSensitivity * playerAiming.sensitivityMultiplier) / 2.1f;
        // Capture the initial mouse position
        lastMouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        lastMouseY = (Input.GetAxis("Mouse Y") * playerAiming.verticalSensitivity  * playerAiming.sensitivityMultiplier) / 2.1f;
    }

    void Update()
    {
        // Increment the timer by the time passed since the last frame
        float mouseX = Input.GetAxis("Mouse X") ;
        
        // Accumulate the angle change
        accumulatedAngle += mouseX;
        
        // Track time
        timer += Time.deltaTime;

        if(timer >= updateInterval)
        {
            CalculateMouseAngleChange();
        }

        //for recording the angle change of an attempt
        if (!isAttemptActive && angleChanges.Count > 0) // When attempt is finished, calculate average.
        {
            float attemptAngleChange = CalculateAttemptAngleChange();
            Debug.Log("Average Angle Change: " + attemptAngleChange);
            angleChanges = new List<float>(); // Reset for the next attempt.
        }
    }

    void CalculateMouseAngleChange()
    {

        angleChangePerInterval = accumulatedAngle / timer;

            // Output the angle change for the interval
        //Debug.Log("Angle Change Per Interval: " + angleChangePerInterval);
        if(isAttemptActive)
        {
            //Debug.Log("Angle Change: " + accumulatedAngle);
            angleChanges.Add(accumulatedAngle);
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
            //Debug.Log("Change: " + change);
            totalChange += change;
        }
        //Debug.Log("Total Change: " + totalChange);
        return totalChange;
    }

    void DisplayMouseAngleChange()
    {
        // Display the mouse angle change on the UI (TextMeshPro)
        if (angleText != null)
        {
            // Display X and Y axis change (or just one if you prefer)
            angleText.text = "Mouse Angle Change: X = " + accumulatedAngle.ToString("F2") + "°/s";
        }
    }
}
