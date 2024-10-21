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

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
