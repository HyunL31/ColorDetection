using UnityEngine;

/// <summary>
/// This class have index of material, what it wants to change.
/// Prefab's paintable part has this class with material index.
/// When user touchs the part in coloring phase, this colormanager 
/// and answercolorlist use this class for painting materials.
/// </summary>
public class PaintablePart : MonoBehaviour
{
    public int matIndex = 0;

    public int GetMatIndex()
    {
        return matIndex;
    }
}
