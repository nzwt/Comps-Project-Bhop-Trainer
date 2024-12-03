using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StrafingStatScreen : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject statScreen;
    public JumpAttempt lastJumpAttempt;
    public JumpAttempt currentJumpAttempt;
    //text boxes
    public TextMeshProUGUI timingOffset;
    public TextMeshProUGUI strafeTimingOffset;
    public TextMeshProUGUI StrafingTendency;
    public TextMeshProUGUI totalScore;
    public bool isFirstTime = true;
    // Start is called before the first frame update
    void Start()
    {
        //lastJumpAttempt = gameManager.GetComponent<GameManager>().lastJumpAttempt;
    }

    public void updateStats()
    {
        if(lastJumpAttempt.scenarioNumber == currentJumpAttempt.scenarioNumber)
        {
            
        }
        if(currentJumpAttempt.strafeTimingOffset < 0)
        {
            StrafingTendency.text = "Early";
        }
        else
        {
            StrafingTendency.text = "Late";
        }
        float currentStrafeTimingOffset = Math.Abs(currentJumpAttempt.strafeTimingOffset);
        //changeInAngle.text = "Change in Angle: " + lastJumpAttempt.angle.ToString("F2") + " degrees";
        if(lastJumpAttempt.strafeTimingOffset == float.NaN)
        {
            if(currentStrafeTimingOffset == float.NaN)
            {
                strafeTimingOffset.text = "None (" + lastJumpAttempt.strafeTimingOffset.ToString("F2") + "None▲\\▼)";
                strafeTimingOffset.color = Color.red;
            }
            else
            {
                strafeTimingOffset.text = currentStrafeTimingOffset.ToString("F2") + " (None▲\\▼)";
                strafeTimingOffset.color = Color.green;
            }
        }
        else if(currentStrafeTimingOffset == float.NaN)
        {
            strafeTimingOffset.text = "None (" + lastJumpAttempt.strafeTimingOffset.ToString("F2") + "▲\\▼)";
            strafeTimingOffset.color = Color.green;
        }
        else if(Math.Abs(lastJumpAttempt.strafeTimingOffset) >= currentStrafeTimingOffset)
        {
            strafeTimingOffset.text = currentStrafeTimingOffset.ToString("F2") + " (" + lastJumpAttempt.strafeTimingOffset.ToString("F2") + "▲)";
            strafeTimingOffset.color = Color.green;
        }
        else
        {
            strafeTimingOffset.text = currentStrafeTimingOffset.ToString("F2") + " (" + lastJumpAttempt.strafeTimingOffset.ToString("F2") + "▼)";
            strafeTimingOffset.color = Color.red;
        }
        if( Math.Abs(lastJumpAttempt.lookOffset)  >= Math.Abs(currentJumpAttempt.lookOffset) )
        {
            timingOffset.text = currentJumpAttempt.lookOffset.ToString("F2") + " (" + lastJumpAttempt.lookOffset.ToString("F2") + "▲)";
            timingOffset.color = Color.green;
        }
        else
        {
            timingOffset.text = currentJumpAttempt.lookOffset.ToString("F2") + " (" + lastJumpAttempt.lookOffset.ToString("F2") + "▼)";
            timingOffset.color = Color.red;
        }
        //totalScore.text = "Total Score: " + lastJumpAttempt.score.ToString();
        if(lastJumpAttempt.score <= currentJumpAttempt.score)
        {
            totalScore.text = currentJumpAttempt.score.ToString("F2") + " (" + lastJumpAttempt.score.ToString("F2") + "▲)";
            totalScore.color = Color.green;
        }
        else
        {
            totalScore.text = currentJumpAttempt.score.ToString("F2") + " (" + lastJumpAttempt.score.ToString("F2") + "▼)";
            totalScore.color = Color.red;
        }
    }
}