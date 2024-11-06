using UnityEngine;
using UnityEngine.UI;

public class TimelineController : MonoBehaviour
{
    [Header("Timeline Setup")]
    public RectTransform timelinePanel; // Reference to your timeline panel
    public GameObject pipPrefab; // Prefab for the pips

    [Header("Event Data")]
    public float[] jumpTimestamps; // Array of jump event timestamps
    public float[] strafeStartTimestamps; // Array of strafe event timestamps
    public float[] strafeEndTimestamps; // Array of strafe event timestamps
    public float[] startLookTimestamps; // Array of look event timestamps
    public float[] endLookTimestamps; // Array of look event timestamps

    [Header("Pip Colors")]
    public Color jumpColor = Color.green;
    public Color strafeColor = Color.blue;
    public Color lookColor = Color.red;
    [Header("Pip Offset")]
    public float xOffset = -150; // Offset to adjust the position of the pips on the x-axis

    public void CalculateTimestamps()
    {
        // Calculate the total duration of the attempt
        float totalDuration = Mathf.Max(
            GetLastTimestamp(jumpTimestamps),
            GetLastTimestamp(strafeStartTimestamps),
            GetLastTimestamp(strafeEndTimestamps),
            GetLastTimestamp(startLookTimestamps),
            GetLastTimestamp(endLookTimestamps)
        );

        // Create pips for each event type
        CreatePips(jumpTimestamps, jumpColor, totalDuration);
        CreatePips(strafeStartTimestamps, strafeColor, totalDuration);
        CreatePips(strafeEndTimestamps, strafeColor, totalDuration);
        CreatePips(startLookTimestamps, lookColor, totalDuration);
        CreatePips(endLookTimestamps, lookColor, totalDuration);
    }

    float GetLastTimestamp(float[] timestamps)
    {
        if (timestamps == null || timestamps.Length == 0) return 0f;
        return timestamps[timestamps.Length - 1];
    }

    void CreatePips(float[] timestamps, Color color, float totalDuration)
    {
        if (timestamps == null || timestamps.Length == 0) return;

        foreach (float timestamp in timestamps)
        {
            if(timestamp == -1000)
            {
                continue;
            }
            // Instantiate the pip and set its color
            GameObject pip = Instantiate(pipPrefab, timelinePanel);
            pip.GetComponent<Image>().color = color;

            // Set the position of the pip on the timeline
            RectTransform rectTransform = pip.GetComponent<RectTransform>();
            float positionX = timestamp / totalDuration * timelinePanel.rect.width + xOffset;
            rectTransform.anchoredPosition = new Vector2(positionX, 0);
            rectTransform.position = rectTransform.position + new Vector3(xOffset/2, 0, 0);
            
        }
    }
    public void RemoveAllPips()
    {
        // Remove all child objects from the timeline panel
        foreach (Transform child in timelinePanel)
        {
            Destroy(child.gameObject);
        }
    }
}
