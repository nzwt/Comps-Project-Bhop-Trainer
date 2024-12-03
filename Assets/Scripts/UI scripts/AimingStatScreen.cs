using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AimingStatScreen : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject statScreen;
    public JumpAttempt lastJumpAttempt;
    public JumpAttempt currentJumpAttempt;
    //text boxes
    public TextMeshProUGUI aimSmoothness;
    public TextMeshProUGUI bhopAccuracy;
    public TextMeshProUGUI totalScore;
    // Start is called before the first frame update
    void Start()
    {
        //lastJumpAttempt = gameManager.GetComponent<GameManager>().lastJumpAttempt;
    }

    public void updateStats()
    {
        if(lastJumpAttempt.aimSmoothness >= currentJumpAttempt.aimSmoothness)
        {
            aimSmoothness.text =currentJumpAttempt.aimSmoothness.ToString("F2") + " (" + lastJumpAttempt.aimSmoothness.ToString("F2") + "▲)";
            aimSmoothness.color = Color.green;
        }
        else
        {
            aimSmoothness.text =currentJumpAttempt.aimSmoothness.ToString("F2") + " (" + lastJumpAttempt.aimSmoothness.ToString("F2") + "▼)";
            aimSmoothness.color = Color.red;
        }
        if( Math.Abs(lastJumpAttempt.lookOffset)  <= Math.Abs(currentJumpAttempt.lookOffset) )
        {
            bhopAccuracy.text = currentJumpAttempt.lookOffset.ToString("F2") + " (" + lastJumpAttempt.lookOffset.ToString("F2") + "▲)";
            bhopAccuracy.color = Color.green;
        }
        else
        {
            bhopAccuracy.text = currentJumpAttempt.lookOffset.ToString("F2") + " (" + lastJumpAttempt.lookOffset.ToString("F2") + "▼)";
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