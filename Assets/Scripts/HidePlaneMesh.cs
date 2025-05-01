using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// This class manages hiding, showing, and resetting AR planes detected by ARPlaneManager.
/// Useful for controlling plane visuals and restarting AR sessions.
/// </summary>
public class HidePlaneMesh : MonoBehaviour
{
    [SerializeField] private ARPlaneManager planeManager; // Manages detection and tracking of AR planes
    [SerializeField] private ARSession arSession; // Handles the AR session state

    /// <summary>
    /// Hides all currently detected planes and disables plane detection.
    /// </summary>
    public void HideAllPlanes()
    {
        planeManager.enabled = false;
        
        foreach (var plane in planeManager.trackables)
        {
            if (plane == null) continue;

            var meshRenderer = plane.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }

            var meshVisualizer = plane.GetComponent<ARPlaneMeshVisualizer>();
            if (meshVisualizer != null)
            {
                meshVisualizer.enabled = false;
            }

            var lineRenderer = plane.GetComponent<LineRenderer>();
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
        }
        
    }

    /// <summary>
    /// Makes all previously detected planes visible again and resumes plane detection.
    /// </summary>
    public void ShowAllPlanes()
    {
        planeManager.enabled = true;
        
        foreach (var plane in planeManager.trackables)
        {
            if (plane == null) continue;

            var meshRenderer = plane.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = true;
            }

            var meshVisualizer = plane.GetComponent<ARPlaneMeshVisualizer>();
            if (meshVisualizer != null)
            {
                meshVisualizer.enabled = true;
            }

            var lineRenderer = plane.GetComponent<LineRenderer>();
            if (lineRenderer != null)
            {
                lineRenderer.enabled = true;
            }
        }
        
    }

    /// <summary>
    /// Destroys all detected planes and resets the AR session.
    /// Useful when restarting the experience from scratch.
    /// </summary>
    public void ResetPlane()
    {
        foreach (var plane in planeManager.trackables)
        {
            Destroy(plane.gameObject);
        }
        arSession.Reset();
    }
}