using UnityEngine;

[ExecuteInEditMode]
// This script will draw the bounds of the renderer in the scene view
public class DrawRendererBounds : MonoBehaviour
{
    public Renderer targetRenderer; // drag the target renderer here

    void OnDrawGizmos()
    {
        if (targetRenderer)
        {
            Gizmos.color = Color.green; // set the color
            Bounds bounds = targetRenderer.bounds; // get the bounds
            Gizmos.DrawWireCube(bounds.center, bounds.size); // draw the bounds
        }
    }
}
