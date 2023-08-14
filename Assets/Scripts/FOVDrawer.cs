using UnityEngine;

public class FOVDrawer : MonoBehaviour
{
    private Mesh _mesh;
    
    public float fov = 90f;
    public float distance = 5f;
    public int resolution = 50;
    public float yOffset = -0.807f;
    public LayerMask layerMask;
    
    private void Start()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
    }

    private void Update()
    {
        var origin = new Vector3(0, yOffset, 0);
        var angle = 0f;
        var angleIncrease = fov / resolution;

        var vertices = new Vector3[resolution + 1 + 1];
        var uv = new Vector2[vertices.Length];
        var triangles = new int[resolution * 3];

        vertices[0] = origin;

        var vertexIndex = 1;
        var triangleIndex = 0;
        for (var i = 0; i <= resolution; i++)
        {
            var vertex = Vector3.zero;

            var position = transform.parent.position;
            var direction = Quaternion.Euler(0, angle - fov + 180, 0) * transform.forward;
            
            Physics.Raycast(position, direction, out RaycastHit hitInfo, distance, layerMask);
            vertex = transform.parent.InverseTransformPoint(hitInfo.collider == null ? position + direction * distance : hitInfo.point);
            
            vertices[vertexIndex] = origin + vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = 0;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        _mesh.vertices = vertices;
        _mesh.uv = uv;
        _mesh.triangles = triangles;
    }
}