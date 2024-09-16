using UnityEngine;

public class CurvedArrow3D : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int segments = 50;  // Number of points in the curve
    public float radius = 2.0f;  // Radius of the curve

    public float xOffset = 0.0f;  // Offset along the X-axis
    public float zOffset = 0.0f;  // Offset along the Y-axis
    //public GameObject arrowheadPrefab;  // Prefab for the 3D arrowhead

    void Start()
    {
        lineRenderer.positionCount = segments + 1;
        CreateArc(45f);  // Create a 45-degree curved path
        InstantiateArrowhead();
    }

    void CreateArc(float angle)
    {
        float angleStep = Mathf.Deg2Rad * (angle / segments);  // Convert degrees to radians
        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = angleStep * i;
            float x = (Mathf.Cos(currentAngle) * radius) + xOffset;  // X-axis for the curve
            float z = (Mathf.Sin(currentAngle) * radius) + zOffset;  // Z-axis for the curve

            lineRenderer.SetPosition(i, new Vector3(x, 1, z));  // Set points along the XZ plane
        }
    }

    void InstantiateArrowhead()
    {
        // Place the arrowhead at the end of the curve
        Vector3 arrowheadPosition = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
        Quaternion arrowheadRotation = Quaternion.LookRotation(lineRenderer.GetPosition(lineRenderer.positionCount - 1) - lineRenderer.GetPosition(lineRenderer.positionCount - 2));
        //Instantiate(arrowheadPrefab, arrowheadPosition, arrowheadRotation);
    }
}
