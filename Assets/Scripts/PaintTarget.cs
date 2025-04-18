using UnityEngine;

public class PaintTarget : MonoBehaviour
{
    public PaintablePart targetMaterial;
    public int index;

    public void Paint(Color color)
    {
        if (targetMaterial != null)
        {
            targetMaterial.SetColorAtIndex(index, color);
        }
    }

    public Color GetCurrentColor()
    {
        if (targetMaterial != null)
        {
            return targetMaterial.GetColorAtIndex(index);
        }
        return Color.clear;
    }
}