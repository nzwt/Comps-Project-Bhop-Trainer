using UnityEngine;
using TMPro;

public class YAxisSpeedTracker : MonoBehaviour
{
    public TextMeshProUGUI speedText;   // TextMeshProUGUI to display speed

    private float lastZPosition;
    private float speed;
    public float updateInterval = 0.1f; // Update interval (0.1 seconds)
    private float timer = 0f;

    void Start()
    {
        // Store the initial Y position of the object
        lastZPosition = transform.position.y;
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
    }

    void CalculateZSpeed()
    {
        // Calculate speed based on the Y-axis distance traveled
        float currentZPosition = transform.position.z;
        float distanceY = currentZPosition - lastZPosition;
        speed = distanceY / updateInterval; // Speed = distance / time (0.1 seconds in this case)

        // Update the last Y position
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
}
