using System;
using Unity.Profiling.LowLevel.Unsafe;

[System.Serializable]
public class JumpAttempt
{
    public int scenarioNumber;
    public int attemptNumber;
    public float strafeTimingOffset;
    public float time;
    public float aimSmoothness;
    public float height;
    public float speed;
    public float score;
    public float angle;
    public float lookOffset;
    public float bhopAccuracy;
    public DateTime date;

    public JumpAttempt(int scenarioNumber, int attemptNumber, float strafeTimingOffset, float time, float smoothness, float height, float speed, float score, float angle, float lookOffset, float bhopAccuracy, DateTime date)
    {
        this.scenarioNumber = scenarioNumber;
        this.attemptNumber = attemptNumber;
        this.strafeTimingOffset = strafeTimingOffset;
        this.time = time;
        this.aimSmoothness = smoothness;
        this.height = height;
        this.speed = speed;
        this.score = score;
        this.angle = angle;
        this.lookOffset = lookOffset;
        this.bhopAccuracy = bhopAccuracy;
        this.date = date;
    }
}

