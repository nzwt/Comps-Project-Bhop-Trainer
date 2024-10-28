using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatScreen : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject statScreen;
    public JumpAttempt lastJumpAttempt;
    public JumpAttempt currentJumpAttempt;
    //text boxes
    public TextMeshProUGUI averageSpeed;
    public TextMeshProUGUI changeInAngle;
    public TextMeshProUGUI aimSmoothness;
    public TextMeshProUGUI totalScore;
    // Start is called before the first frame update
    void Start()
    {
        //lastJumpAttempt = gameManager.GetComponent<GameManager>().lastJumpAttempt;
    }

    public void updateStats()
    {
        // currentJumpAttempt = gameManager.GetComponent<GameManager>().currentJumpAttempt;
        // lastJumpAttempt = gameManager.GetComponent<GameManager>().lastJumpAttempt;
        //averageSpeed.text = "Average Speed: " + currentJumpAttempt.speed.ToString("F2") + " (" + lastJumpAttempt.speed.ToString("F2") + ")";
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
        // if(lastJumpAttempt.aimSmoothness <= currentJumpAttempt.aimSmoothness)
        // {
        //     aimSmoothness.text =currentJumpAttempt.aimSmoothness.ToString("F2") + " (" + lastJumpAttempt.aimSmoothness.ToString("F2") + "▲)";
        //     aimSmoothness.color = Color.green;
        // }
        // else
        // {
        //     aimSmoothness.text =currentJumpAttempt.aimSmoothness.ToString("F2") + " (" + lastJumpAttempt.aimSmoothness.ToString("F2") + "▼)";
        //     aimSmoothness.color = Color.red;
        // }
        //aimSmoothness.text = "Aim Smoothness: " + lastJumpAttempt.aimSmoothness.ToString("F2") + " degrees";
        //check which is closer to 45
        if( Math.Abs(lastJumpAttempt.angle - 45) > Math.Abs(currentJumpAttempt.angle - 45))
        {
            changeInAngle.text =currentJumpAttempt.angle.ToString("F2") + "° (" + lastJumpAttempt.angle.ToString("F2") + "°▲)";
            changeInAngle.color = Color.green;
        }
        else
        {
            changeInAngle.text = currentJumpAttempt.angle.ToString("F2") + "° (" + lastJumpAttempt.angle.ToString("F2") + "°▼)";
            changeInAngle.color = Color.red;
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
