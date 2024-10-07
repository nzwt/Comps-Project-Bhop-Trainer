using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BhopStatScreen : MonoBehaviour
{
public GameObject gameManager;
    public GameObject statScreen;
    public JumpAttempt lastJumpAttempt;
    public JumpAttempt currentJumpAttempt;
    //text boxes
    public TextMeshProUGUI averageSpeed;
    public TextMeshProUGUI bhopAccuracy;
    public TextMeshProUGUI totalScore;
    // Start is called before the first frame update
    void Start()
    {
        //lastJumpAttempt = gameManager.GetComponent<GameManager>().lastJumpAttempt;
    }

    public void updateStats()
    {
        if( lastJumpAttempt.speed  <= currentJumpAttempt.speed )
        {
            averageSpeed.text = currentJumpAttempt.speed.ToString("F2") + " (" + lastJumpAttempt.speed.ToString("F2") + "▲)";
            averageSpeed.color = Color.green;
        }
        else
        {
            averageSpeed.text = currentJumpAttempt.speed.ToString("F2") + " (" + lastJumpAttempt.speed.ToString("F2") + "▼)";
            averageSpeed.color = Color.red;
        }
        //changeInAngle.text = "Change in Angle: " + lastJumpAttempt.angle.ToString("F2") + " degrees";
        if(lastJumpAttempt.bhopAccuracy <= currentJumpAttempt.bhopAccuracy)
        {
            bhopAccuracy.text =currentJumpAttempt.bhopAccuracy.ToString("F2") + " (" + lastJumpAttempt.bhopAccuracy.ToString("F2") + "▲)";
            bhopAccuracy.color = Color.green;
        }
        else
        {
            bhopAccuracy.text =currentJumpAttempt.bhopAccuracy.ToString("F2") + " (" + lastJumpAttempt.bhopAccuracy.ToString("F2") + "▼)";
            bhopAccuracy.color = Color.red;
        }
        //totalScore.text = "Total Score: " + lastJumpAttempt.score.ToString();
        if(lastJumpAttempt.score <= currentJumpAttempt.score)
        {
            totalScore.text =currentJumpAttempt.score.ToString("F2") + " (" + lastJumpAttempt.score.ToString() + "▲)";
            totalScore.color = Color.green;
        }
        else
        {
            totalScore.text = currentJumpAttempt.score.ToString("F2") + " (" + lastJumpAttempt.score.ToString() + "▼)";
            totalScore.color = Color.red;
        }
    }
}
