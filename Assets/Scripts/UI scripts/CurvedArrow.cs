using UnityEngine;

public class CurvedArrow : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int segments = 50;  // The number of points in the arc
    public float radius = 2.0f;  // Radius of the arc

    void Start()
    {
        lineRenderer.positionCount = segments + 1;
        DrawArc(45);  // Draws an arc representing the optimal 45-degree angle
    }

    void DrawArc(float angle)
    {
        float angleStep = angle / segments;  // How much the angle increases per segment
        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = Mathf.Deg2Rad * angleStep * i;
            float x = Mathf.Cos(currentAngle) * radius;
            float y = Mathf.Sin(currentAngle) * radius;

            lineRenderer.SetPosition(i, new Vector3(x, y, 0));  // Plot the points in 2D space
        }
    }
}
