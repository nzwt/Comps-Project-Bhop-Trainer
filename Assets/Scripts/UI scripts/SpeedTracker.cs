using UnityEngine;
using TMPro;

public class SpeedTracker : MonoBehaviour
{
    public TextMeshProUGUI speedText;  // TextMeshProUGUI to display speed

    private Vector3 lastPosition;
    private float speed;
    private float updateInterval = 0.1f; // Interval for updating speed (0.1 seconds)
    private float timer = 0f;            // Timer to track time
    private float attemptTimer = 0f;     // Timer to track time for an attempt
    private float totalSpeed = 0f;       // Total speed for calculating average
    public bool isAttemptActive = false;
    public float currentAttemptSpeed = 0f;
    

    void Start()
    {
        lastPosition = transform.position; // Store the initial position
    }

    void Update()
    {
        // Increment the timer by the time that has passed since the last frame
        timer += Time.deltaTime;

        // If 0.1 seconds have passed, calculate and update the speed
        if (timer >= updateInterval)
        {
            CalculateSpeed();
            DisplaySpeed();

            // Reset the timer
            timer = 0f;
        }

        if(isAttemptActive && speed > 0)
        {
            totalSpeed += speed;
            attemptTimer += Time.deltaTime;
        }
        else if(!isAttemptActive && totalSpeed > 0)
        {
            float attemptSpeed = CalculateAttemptSpeed();
            Debug.Log("Average Speed: " + attemptSpeed);
            currentAttemptSpeed = attemptSpeed;
            totalSpeed = 0f;
            attemptTimer = 0f;
        }
    }

    void CalculateSpeed()
    {
        // Calculate speed based on the distance traveled between the last frame and current frame
        float distance = Vector3.Distance(lastPosition, transform.position);
        speed = distance / updateInterval; // Speed = distance / time (0.1 seconds in this case)

        lastPosition = transform.position; // Update last position
    }

    public float CalculateAttemptSpeed()
    {
        float attemptSpeed = totalSpeed / (timer / updateInterval);
        return attemptSpeed;
    }

    void DisplaySpeed()
    {
        // Display speed in the TextMeshPro text component
        if (speedText != null)
        {
            speedText.text = "Speed: " + speed.ToString("F2") + " units/s";
        }
    }
}
