using UnityEngine;
using TMPro;

public class YAxisSpeedTracker : MonoBehaviour
{
    public TextMeshProUGUI speedText;   // TextMeshProUGUI to display speed

    private float lastYPosition;
    private float speed;
    public float updateInterval = 0.1f; // Update interval (0.1 seconds)
    private float timer = 0f;

    void Start()
    {
        // Store the initial Y position of the object
        lastYPosition = transform.position.y;
    }

    void Update()
    {
        // Increment the timer by the time that has passed since the last frame
        timer += Time.deltaTime;

        // Update every 0.1 seconds
        if (timer >= updateInterval)
        {
            CalculateXSpeed();
            DisplaySpeed();

            // Reset the timer
            timer = 0f;
        }
    }

    void CalculateXSpeed()
    {
        // Calculate speed based on the Y-axis distance traveled
        float currentYPosition = transform.position.y;
        float distanceY = currentYPosition - lastYPosition;
        speed = distanceY / updateInterval; // Speed = distance / time (0.1 seconds in this case)

        // Update the last Y position
        lastYPosition = currentYPosition;
    }

    void DisplaySpeed()
    {
        // Display the speed in the TextMeshPro text component
        if (speedText != null)
        {
            speedText.text = "Speed (Y): " + speed.ToString("F2") + " units/s";
        }
    }
}
