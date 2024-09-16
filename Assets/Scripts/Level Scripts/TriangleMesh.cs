using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TriangleMesh : MonoBehaviour
{
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        // Define the vertices of the triangle
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0, 1, 0),  // Top vertex
            new Vector3(-1, -1, 0), // Bottom left vertex
            new Vector3(1, -1, 0)   // Bottom right vertex
        };

        // Define the triangle (which vertices form the triangle)
        int[] triangles = new int[]
        {
            0, 1, 2  // The three vertices that form the triangle
        };

        // Optionally, define normals and UVs
        Vector3[] normals = new Vector3[]
        {
            Vector3.back, Vector3.back, Vector3.back
        };

        Vector2[] uv = new Vector2[]
        {
            new Vector2(0.5f, 1),   // UV for top vertex
            new Vector2(0, 0),      // UV for bottom left vertex
            new Vector2(1, 0)       // UV for bottom right vertex
        };

        // Assign vertices, triangles, normals, and UVs to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;

        // Assign the mesh to the MeshFilter
        meshFilter.mesh = mesh;
    }
}
