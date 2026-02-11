using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class OutlinePositionUpdater : MonoBehaviour
{
    // Assign your Forward Renderer Data asset here if needed, 
    // or just ensure the material is accessible.
    public Material outlineMaterial; 
    public float radius = 5f;

    void Update()
    {
        if (outlineMaterial != null)
        {
            // Update the center position to this object's position
            outlineMaterial.SetVector("_Sphere_Center", transform.position);
            outlineMaterial.SetFloat("_Sphere_Radius", radius);
        }
    }
    
    // Optional: Draw a gizmo to see the sphere in Editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}