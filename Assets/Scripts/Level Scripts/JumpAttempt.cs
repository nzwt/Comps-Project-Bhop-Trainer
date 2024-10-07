using System;
using Unity.Profiling.LowLevel.Unsafe;

[System.Serializable]
public class JumpAttempt
{
    public int scenarioNumber;
    public int attemptNumber;
    public float jumpForce;
    public float time;
    public float distance;
    public float height;
    public float speed;
    public float score;
    public float angle;
    public float aimSmoothness;
    public float bhopAccuracy;
    public DateTime date;

    public JumpAttempt(int scenarioNumber, int attemptNumber, float jumpForce, float time, float distance, float height, float speed, float score, float angle, float aimSmoothness, float bhopAccuracy, DateTime date)
    {
        this.scenarioNumber = scenarioNumber;
        this.attemptNumber = attemptNumber;
        this.jumpForce = jumpForce;
        this.time = time;
        this.distance = distance;
        this.height = height;
        this.speed = speed;
        this.score = score;
        this.angle = angle;
        this.aimSmoothness = aimSmoothness;
        this.bhopAccuracy = bhopAccuracy;
        this.date = date;
    }
}

