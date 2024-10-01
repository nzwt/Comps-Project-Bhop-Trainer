using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public List<JumpAttempt> scoreList = new List<JumpAttempt>();
    public bool isLoaded = false;

    private string filePath;

    void Start()
    {
        // Set the file path to save in a persistent location
        filePath = Path.Combine(Application.persistentDataPath, "JumpAttempt.json");

        Debug.Log("File path: " + filePath);

        // Load scores if file exists
        LoadScores();
    }

    public void SaveScore(int attemptNumber, float jumpForce, float time, float distance, float height, float speed, float score, float angle, float aimSmoothness)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("File path is null or empty!");
            return;
        }

        // Create a new score entry
        JumpAttempt newScore = new JumpAttempt(attemptNumber, jumpForce, time, distance, height, speed, score, angle, aimSmoothness, date: System.DateTime.Now);

        // Add the new score to the list
        scoreList.Add(newScore);

        // Serialize the score list to JSON
        string json = JsonUtility.ToJson(new ScoreListWrapper(scoreList), true);

        // Write the JSON string to the file
        File.WriteAllText(filePath, json);

        Debug.Log("Score saved to: " + filePath);
    }

    public void SaveScore(JumpAttempt newScore)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("File path is null or empty!");
            return;
        }

        // Add the new score to the list
        scoreList.Add(newScore);

        // Serialize the score list to JSON
        string json = JsonUtility.ToJson(new ScoreListWrapper(scoreList), true);

        // Write the JSON string to the file
        File.WriteAllText(filePath, json);

        Debug.Log("Score saved to: " + filePath);
    }

    public void LoadScores()
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("File path is null or empty!");
            //Debug.Break();
            return;
        }

        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read the JSON from the file
            string json = File.ReadAllText(filePath);

            // Deserialize the JSON into the score list
            ScoreListWrapper loadedData = JsonUtility.FromJson<ScoreListWrapper>(json);
            scoreList = loadedData.scores;

            Debug.Log("Scores loaded from file.");
        }
        else
        {
            Debug.Log("No save file found, starting fresh.");
        }
        isLoaded = true;
    }

    public JumpAttempt GetLastJumpAttempt()
    {
        if (scoreList.Count > 0)
        {
            return scoreList[scoreList.Count - 1];
        }
        else
        {
            return new JumpAttempt(0, 0, 0, 0, 0, 0, 0, 0, 0, date: System.DateTime.Now);;
        }
    }

    // Wrapper class for holding a list of JumpAttempt
    [System.Serializable]
    private class ScoreListWrapper
    {
        public List<JumpAttempt> scores;

        public ScoreListWrapper(List<JumpAttempt> scores)
        {
            this.scores = scores;
        }
    }
}
