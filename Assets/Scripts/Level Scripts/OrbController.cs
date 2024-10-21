using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour
{
    public GameObject leftOrb;
    public GameObject rightOrb;
    public bool isLeft = true;

    // Start is called before the first frame update
    public void TargetRight()
    {
        rightOrb.GetComponent<Renderer>().material.color = Color.green;
        leftOrb.GetComponent<Renderer>().material.color = Color.white;
    }

    public void TargetLeft()
    {
        leftOrb.GetComponent<Renderer>().material.color = Color.green;
        rightOrb.GetComponent<Renderer>().material.color = Color.white;
    }
    public void switchTarget()
    {
        if (isLeft)
        {
            TargetRight();
            isLeft = false;
        }
        else
        {
            TargetLeft();
            isLeft = true;
        }
    }
    public void resetTargets()
    {
        leftOrb.GetComponent<Renderer>().material.color = Color.white;
        rightOrb.GetComponent<Renderer>().material.color = Color.white;
    }
    public void ShowRightAccuracy(float time)
    {
        if(Math.Abs(0.65-time) < 0.1)
        {
            rightOrb.GetComponent<Renderer>().material.color = Color.green;
        }
        else if (Math.Abs(0.65 - time) < 0.2)
        {
            rightOrb.GetComponent<Renderer>().material.color = Color.yellow;
        }
        else
        {
            rightOrb.GetComponent<Renderer>().material.color = Color.red;
        }
        leftOrb.GetComponent<Renderer>().material.color = Color.white;
    }
    public void ShowLeftAccuracy(float time)
    {
        if (Math.Abs(0.65 - time) < 0.1)
        {
            leftOrb.GetComponent<Renderer>().material.color = Color.green;
        }
        else if (Math.Abs(0.65 - time) < 0.2)
        {
            leftOrb.GetComponent<Renderer>().material.color = Color.yellow;
        }
        else
        {
            leftOrb.GetComponent<Renderer>().material.color = Color.red;
        }
        rightOrb.GetComponent<Renderer>().material.color = Color.white;
    }   

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
