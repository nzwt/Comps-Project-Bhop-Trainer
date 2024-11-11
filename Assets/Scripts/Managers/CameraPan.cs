using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    // Controls the speed of the rotation
    public float rotationSpeed = 10f;
    // Rotation axis, default to rotate around the Y-axis
    public Vector3 rotationAxis = Vector3.up;

    void Update()
    {
        // Rotate the camera around the specified axis at the given speed
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}
