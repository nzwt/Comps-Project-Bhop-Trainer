using System.Collections;
using System.Collections.Generic;
using Fragsurf.Movement;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // refrences to other scripts
    public SurfCharacter surfCharacter;
    public MouseAngleTracker mouseAngleTracker;
    public SpeedTracker speedTracker;
    public PlayerAiming playerAiming;
    public void ResetPlayer( float x, float y, float z)
    {
        // Reset the player's position and rotation
        surfCharacter.transform.position = new Vector3(x, y, z);
        surfCharacter.transform.rotation = Quaternion.LookRotation(Vector3.forward);
        //disable input
        surfCharacter.moveData.verticalAxis = 0;
        surfCharacter.moveData.horizontalAxis = 0;
        surfCharacter.movementEnabled = false;
        mouseAngleTracker.isAttemptActive = false;
        speedTracker.isAttemptActive = false;
        DisableMouseLook();
    }

    public void DisableMouseLook()
    {
        playerAiming.resetRotation();
        playerAiming.canAim = false;
    }

    public void EnableMouseLook()
    {
        playerAiming.canAim = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
