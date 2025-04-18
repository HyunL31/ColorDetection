using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class HidePlaneMesh : MonoBehaviour
{
    public ARPlaneManager planeManager;

    void Start()
    {
        if (planeManager == null)
        {
            planeManager = FindAnyObjectByType<ARPlaneManager>();
        }
    }

    void Update()
    {
        HideAllPlanes();
    }

    void HideAllPlanes()
    {
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
}