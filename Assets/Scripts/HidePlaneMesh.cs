using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class HidePlaneMesh : MonoBehaviour
{
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private ARSession arSession;

    public void HideAllPlanes()
    {
        planeManager.enabled = false;
        
        foreach (var plane in planeManager.trackables)
        {
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

    public void ShowAllPlanes()
    {
        planeManager.enabled = true;
        
        foreach (var plane in planeManager.trackables)
        {
            var meshRenderer = plane.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                meshRenderer.enabled = true;
            }

            var meshVisualizer = plane.GetComponent<ARPlaneMeshVisualizer>();
            if (meshVisualizer == null)
            {
                meshVisualizer.enabled = true;
            }

            var lineRenderer = plane.GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer.enabled = true;
            }
        }
        
    }

    public void ResetPlane()
    {
        arSession.Reset();
    }
}