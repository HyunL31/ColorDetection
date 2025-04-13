using UnityEngine;

public class PaintablePart : MonoBehaviour
{
    private Renderer rend;
    private Material[] mats;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            mats = rend.materials;

            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = new Material(mats[i]);  // 인스턴스화
            }

            rend.materials = mats;
        }
    }

    public void SetColorAtIndex(int index, Color color)
    {
        if (rend != null && index >= 0 && index < mats.Length)
        {
            mats[index].color = color;
        }
    }

    public Color GetColorAtIndex(int index)
    {
        if (rend != null && index >= 0 && index < mats.Length)
        {
            return mats[index].color;
        }

        return Color.clear;
    }

    public int GetMaterialCount()
    {
        return mats != null ? mats.Length : 0;
    }
}
