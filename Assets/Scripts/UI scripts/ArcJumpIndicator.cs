using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcJumpIndicator : MonoBehaviour
{
    public RectTransform panel;              // Reference to the panel (box) UI element
    public GameObject dotPrefab;             // Prefab of the dot (UI Image or other UI element)
    public List<GameObject> dots;            // List of dots created during the jump

    public float jumpDuration = 0.65f;       // The estimated time in seconds from jump to ground
    public float apexHeight = 50f;           // Height of the arc in UI units
    public float xOffset = -52f;             // The offset from the left edge of the panel

    private float panelWidth;                // The width of the panel
    private bool isGrounded;                 // Whether the player is on the ground
    private bool isJumping;                  // Whether the player is jumping
    public bool isPaused = false;

    private float currentTime;               // Tracks the time since the jump

    void Start()
    {
        // Calculate the width of the panel and place the static line in the center
        panelWidth = panel.rect.width;

        // Set the static line's position to the center of the box
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            StartJump();
        }


        if (isGrounded && isJumping)
        {
            EndJump();
        }
    }

    public void StartJump()
    {
        isJumping = true;
        currentTime = 0;

        // Create a new dot at the start of the box
        CreateNewDot();
    }

    public void EndJump()
    {
        isJumping = false;
    }

    void CreateNewDot()
    {
        // Instantiate a new dot from the prefab
        GameObject newDot = Instantiate(dotPrefab, panel);
        RectTransform dotRect = newDot.GetComponent<RectTransform>();

        // Set X anchor to the left side of the panel and Y anchor to the middle
        dotRect.anchorMin = new Vector2(0, 0.0f);
        dotRect.anchorMax = new Vector2(0, 0.0f);
        dotRect.pivot = new Vector2(0, 0.0f);

        // Set the initial position of the dot (X offset and Y aligned in the middle)
        float startPositionX = xOffset;
        dotRect.anchoredPosition = new Vector2(startPositionX, 0);

        // Start moving the dot along the arc
        StartCoroutine(MoveDot(dotRect, newDot));
        dots.Add(newDot);  // Add the new dot to the list of dots
    }

    System.Collections.IEnumerator MoveDot(RectTransform dot, GameObject dotObject)
    {
        float elapsedTime = 0f;

        // Continue moving the dot along the arc
        while (elapsedTime < jumpDuration)
        {
            while (isPaused)
            {
                yield return null; // Wait for the next frame while paused
            }
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / jumpDuration);

            // Calculate the horizontal position
            float newX = Mathf.Lerp(0, panelWidth, t);

            // Calculate the vertical position using a quadratic equation
            float newY = -4 * apexHeight * Mathf.Pow(t - 0.5f, 2) + apexHeight;

            // Set the new position of the dot
            dot.anchoredPosition = new Vector2(newX + xOffset, newY);

            yield return null;  // Wait for the next frame
        }

        // Destroy the dot when it completes the jump
        Destroy(dotObject);
    }

    public void deleteDots()
    {
        Debug.Log("Deleting dots");
        foreach (GameObject dot in dots)
        {
            Destroy(dot);
        }
        dots.Clear();
    }

    public void changeLastDotColor(Color color)
    {
        Debug.Log("Changing last dot color");
        if (dots.Count > 0)
        {
            dots[dots.Count - 1].GetComponent<Image>().color = color;
        }
    }
}
