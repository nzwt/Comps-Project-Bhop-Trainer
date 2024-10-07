using UnityEngine;
using UnityEngine.UI;

public class JumpIndicator : MonoBehaviour
{
    public Image movingLine;    // Assign the moving line in the Inspector
    public RectTransform panel; // Assign the panel that contains the indicator
    public float airTime = 0.65f;  // Estimated time player will spend in the air
    private float timeRemaining;

    private bool isJumping = false;
    private float jumpStartTime;
    
    void Update()
    {
        if (isJumping)
        {
            // Calculate how much time has passed since the jump
            float elapsed = Time.time - jumpStartTime;

            // Calculate time remaining before hitting the ground
            timeRemaining = airTime - elapsed;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isJumping = false; // Stop updating once the player hits the ground
            }

            // Update the moving line's position based on the time remaining
            UpdateLinePosition();
        }
    }

    public void StartJump()
    {
        isJumping = true;
        jumpStartTime = Time.time;
        timeRemaining = airTime;
    }

    void UpdateLinePosition()
    {
        // Get the panel's width (or height, depending on the layout)
        float panelWidth = panel.rect.width;

        // Calculate how far along the line should be (0 = start, 1 = finish)
        float progress = Mathf.Clamp01(1f - (timeRemaining / airTime) - 0.52f);

        // Update the moving line's position based on progress
        movingLine.rectTransform.anchoredPosition = new Vector2(progress * panelWidth, 0);
    }
}
