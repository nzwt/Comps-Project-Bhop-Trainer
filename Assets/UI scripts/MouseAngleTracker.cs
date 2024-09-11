using UnityEngine;
using TMPro;
using Fragsurf.Movement;

public class MouseAngleTracker : MonoBehaviour
{
    public TextMeshProUGUI angleText;   // TextMeshProUGUI to display angle change
    public float updateInterval = 0.1f; // Update interval (0.1 seconds)
    public float mouseSensitivity = 1f;  // Sensitivity multiplier to convert input to degrees
    
    public PlayerAiming playerAiming; // Reference to the PlayerAiming script

    private float lastMouseX;
    private float lastMouseY;
    private float angleChangeX;
    private float angleChangeY;
    private float timer = 0f;

    void Start()
    {
        // Capture the initial mouse position
        lastMouseX = (Input.GetAxis("Mouse X") * playerAiming.horizontalSensitivity * playerAiming.sensitivityMultiplier) / 2.1f;
        lastMouseY = (Input.GetAxis("Mouse Y") * playerAiming.verticalSensitivity  * playerAiming.sensitivityMultiplier) / 2.1f;
    }

    void Update()
    {
        // Increment the timer by the time passed since the last frame
        timer += Time.deltaTime;

        // Update every x seconds
        if (timer >= updateInterval)
        {
            CalculateMouseAngleChange();
            DisplayMouseAngleChange();

            // Reset the timer
            timer = 0f;
        }
    }

    void CalculateMouseAngleChange()
    {
        // Calculate change in mouse angle for X-axis (horizontal movement)
        float currentMouseX = Input.GetAxis("Mouse X");
        angleChangeX = (currentMouseX - lastMouseX) * mouseSensitivity;

        // Not currently using y calculation, might be helpful later
        float currentMouseY = Input.GetAxis("Mouse Y");
        angleChangeY = (currentMouseY - lastMouseY) * mouseSensitivity;

        // Update last mouse positions
        lastMouseX = currentMouseX;
        lastMouseY = currentMouseY;
    }

    void DisplayMouseAngleChange()
    {
        // Display the mouse angle change on the UI (TextMeshPro)
        if (angleText != null)
        {
            // Display X and Y axis change (or just one if you prefer)
            angleText.text = "Mouse Angle Change: X = " + angleChangeX.ToString("F2") + "°/s";
        }
    }
}
