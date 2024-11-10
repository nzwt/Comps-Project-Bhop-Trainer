using UnityEngine;
using TMPro;

public class ZAxisSpeedTracker : MonoBehaviour
{
    //THIS IS A Z axis speed tracker
    public TextMeshProUGUI speedText;   // TextMeshProUGUI to display speed

    private float lastZPosition;
    private float speed;
    public float updateInterval = 0.1f; // Update interval (0.1 seconds)
    private float timer = 0f;
    public bool isAttemptActive;
    private float totalSpeed;
    private int speedInstances;

    void Start()
    {
        // Store the initial Z position of the object
        lastZPosition = transform.position.z;
    }

    void Update()
    {
        // Increment the timer by the time that has passed since the last frame
        timer += Time.deltaTime;

        // Update every 0.1 seconds
        if (timer >= updateInterval)
        {
            CalculateZSpeed();
            DisplaySpeed();

            // Reset the timer
            timer = 0f;
        }
        if(isAttemptActive && speed > 0)
        {
            totalSpeed += speed;
            speedInstances++;
        }
    }

    void CalculateZSpeed()
    {
        // Calculate speed based on the Z-axis distance traveled
        float currentZPosition = transform.position.z;
        float distanceZ = currentZPosition - lastZPosition;
        speed = distanceZ / updateInterval; // Speed = distance / time (0.1 seconds in this case)

        // Update the last Z position
        lastZPosition = currentZPosition;
    }

    void DisplaySpeed()
    {
        // Display the speed in the TextMeshPro text component
        if (speedText != null)
        {
            speedText.text = "Speed (Z): " + speed.ToString("F2") + " units/s";
        }
    }
    
    public float calculateAttemptSpeed()
    {
        float attemptSpeed = totalSpeed / speedInstances;
        return attemptSpeed;
    }

    public void reset()
    {
        totalSpeed = 0;
        speedInstances = 0;
    }
}
