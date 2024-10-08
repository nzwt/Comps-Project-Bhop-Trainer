
    // void Update()
    // {
    //     if (isJumping)
    //     {
    //         // Calculate how much time has passed since the jump
    //         float elapsed = Time.time - jumpStartTime;

    //         // Calculate time remaining before hitting the ground
    //         timeRemaining = airTime - elapsed;

    //         if (timeRemaining <= 0)
    //         {
    //             timeRemaining = 0;
    //             isJumping = false; // Stop updating once the player hits the ground
    //         }

    //         // Update the moving line's position based on the time remaining
    //         UpdateLinePosition();
    //     }
    // }

    // public void StartJump()
    // {
    //     isJumping = true;
    //     jumpStartTime = Time.time;
    //     timeRemaining = airTime;
    // }

    // void UpdateLinePosition()
    // {
    //     // Get the panel's width (or height, depending on the layout)
    //     float panelWidth = panel.rect.width + 0.52f;

    //     // Calculate how far along the line should be (0 = start, 1 = finish)
    //     float progress = Mathf.Clamp01(1f - (timeRemaining / airTime) - 0.52f);

    //     // Update the moving line's position based on progress
    //     movingLine.rectTransform.anchoredPosition = new Vector2(progress * panelWidth, 0);
    // }

using UnityEngine;
using UnityEngine.UI;

public class JumpIndicator : MonoBehaviour
{
    public RectTransform panel;              // Reference to the panel (box) UI element
    public RectTransform staticLine;         // Reference to the static line (center) UI element
    public GameObject dynamicLinePrefab;     // Prefab of the dynamic line (UI Image)

    public float jumpDuration = 0.65f;          // The estimated time in seconds from jump to ground
    public float xOffset = -52f;            // The offset from the left edge of the panel

    private float panelWidth;                // The width of the panel
    private float staticLinePosition;        // Position of the static line in the middle of the box
    private bool isGrounded;                 // Whether the player is on the ground
    private bool isJumping;                  // Whether the player is jumping

    private float currentTime;               // Tracks the time since the jump

    // void Start()
    // {
    //     // Calculate the width of the panel and place the static line in the center
    //     panelWidth = panel.rect.width;
    //     staticLinePosition = (panelWidth / 2) - 52;

    //     // Set the static line's position to the center of the box
    //     staticLine.anchoredPosition = new Vector2(staticLinePosition, staticLine.anchoredPosition.y);
    // }

    // void Update()
    // {
    // }

    // public void StartJump()
    // {
    //     isJumping = true;
    //     currentTime = 0;

    //     // Create a new dynamic line at the start of the box
    //     CreateNewDynamicLine();
    // }

    // public void EndJump()
    // {
    //     isJumping = false;
    // }

    // void CreateNewDynamicLine()
    // {
    //     // Instantiate a new dynamic line from the prefab
    //     GameObject newLine = Instantiate(dynamicLinePrefab, panel);

    //     // Reset the position of the new dynamic line to the start of the box
    //     RectTransform newLineRect = newLine.GetComponent<RectTransform>();
    //     newLineRect.anchoredPosition = new Vector2(-52, staticLine.anchoredPosition.y);

    //     // Start moving the line
    //     StartCoroutine(MoveLine(newLineRect));
    // }

    // System.Collections.IEnumerator MoveLine(RectTransform line)
    // {
    //     float elapsedTime = 0f;

    //     // Continue moving the dynamic line from the left to the right of the panel
    //     while (elapsedTime < jumpDuration || line.anchoredPosition.x < panelWidth)
    //     {
    //         elapsedTime += Time.deltaTime;
    //         float progress = Mathf.Clamp01(elapsedTime / jumpDuration);

    //         // Move the line based on the elapsed time
    //         float newPosition = Mathf.Lerp(0, panelWidth, progress);
    //         line.anchoredPosition = new Vector2(newPosition, line.anchoredPosition.y);

    //         yield return null;  // Wait for the next frame
    //     }

    //     // After finishing the movement, the line reaches the end of the box
    //     line.anchoredPosition = new Vector2(panelWidth, line.anchoredPosition.y);
    // }
     void Start()
    {
        // Calculate the width of the panel and place the static line in the center
        panelWidth = panel.rect.width;
        staticLinePosition = (panelWidth / 2) + xOffset;

        // Set the static line's position to the center of the box
        staticLine.anchoredPosition = new Vector2(staticLinePosition, staticLine.anchoredPosition.y);
    }

    void Update()
    {
        // Example condition to detect when the player jumps (replace with your actual jump detection)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            StartJump();
        }

        // Example condition to detect when the player hits the ground (replace with your actual ground detection)
        if (isGrounded && isJumping)
        {
            EndJump();
        }
    }

    public void StartJump()
    {
        isJumping = true;
        currentTime = 0;

        // Create a new dynamic line at the start of the box
        CreateNewDynamicLine();
    }

    public void EndJump()
    {
        isJumping = false;
    }

    void CreateNewDynamicLine()
    {
    // Instantiate a new dynamic line from the prefab
    GameObject newLine = Instantiate(dynamicLinePrefab, panel);

    // Get the RectTransform of the dynamic line
    RectTransform newLineRect = newLine.GetComponent<RectTransform>();

    // Set X anchor to the left side of the panel and Y anchor to the middle
    newLineRect.anchorMin = new Vector2(0, 0.025f);  // X: Left side (0), Y: Middle (0.5)
    newLineRect.anchorMax = new Vector2(0, 0.025f);  // X: Left side (0), Y: Middle (0.5)
    newLineRect.pivot = new Vector2(0, 0.025f);      // Pivot at left (X=0) and middle (Y=0.5)

    // Reset the position of the new dynamic line (X offset and Y aligned in the middle)
    float startPositionX = -52;  // Offset 52 units from the left side
    newLineRect.anchoredPosition = new Vector2(startPositionX, 0);  // Y is 0 to keep it centered vertically

    // Start moving the line
    StartCoroutine(MoveLine(newLineRect, newLine));
    }

    System.Collections.IEnumerator MoveLine(RectTransform line, GameObject lineObject)
    {
        float elapsedTime = 0f;

        // Continue moving the dynamic line from the left to the right of the panel
        while (elapsedTime < jumpDuration || line.anchoredPosition.x < panelWidth)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / (jumpDuration*2));

            // Move the line based on the elapsed time
            float newPosition = Mathf.Lerp(0, panelWidth, progress);
            line.anchoredPosition = new Vector2(newPosition, line.anchoredPosition.y);

            // Check if the line has reached the end of the box
            if (newPosition >= panelWidth)
            {
                Destroy(lineObject);  // Destroy the dynamic line when it reaches the end
                yield break;  // Stop the coroutine
            }

            yield return null;  // Wait for the next frame
        }
    }
}

