using UnityEngine;
using TMPro;

public class XAxisSpeedTracker : MonoBehaviour
{
    public TextMeshProUGUI speedText;   // TextMeshProUGUI to display speed

    private float lastXPosition;
    private float speed;
    public float updateInterval = 0.1f; // Update interval (0.1 seconds)
    private float timer = 0f;

    void Start()
    {
        // Store the initial X position of the object
        lastXPosition = transform.position.x;
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
        // Calculate speed based on the X-axis distance traveled
        float currentXPosition = transform.position.x;
        float distanceX = currentXPosition - lastXPosition;
        speed = distanceX / updateInterval; // Speed = distance / time (0.1 seconds in this case)

        // Update the last X position
        lastXPosition = currentXPosition;
    }

    void DisplaySpeed()
    {
        // Display the speed in the TextMeshPro text component
        if (speedText != null)
        {
            speedText.text = "Speed (X): " + speed.ToString("F2") + " units/s";
        }
    }
}
