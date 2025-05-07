using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages the list of materials (colors) associated with an object or multiple paintable parts.
/// It handles storing correct colors, changing materials to white, painting, and checking correctness.
/// </summary>
public class AnswerColorList : MonoBehaviour
{
    // List of parts that can be painted individually
    [SerializeField] private List<PaintablePart> paintableParts;

    // Optional single GameObject with multiple materials
    [SerializeField] private GameObject mainGO;
    
    // Indices of materials that should be excluded from color detection and painting
    [SerializeField] private int[] exceptionIndex;
    
    // All target materials to paint
    public Material[] targetMaterials;
    
    // Stores the correct colors for each material
    private List<Color> rightColor = new List<Color>();
    private Renderer rend;

    /// <summary>
    /// Initializes and instantiates materials.
    /// If mainGO is set, uses its materials.
    /// Otherwise, collects materials from multiple paintable parts.
    /// </summary>
    public void SetMaterials()
    {
        if(mainGO!=null)
        {
            rend = mainGO.GetComponent<Renderer>();
            targetMaterials = rend.materials;
            for (int i = 0; i < targetMaterials.Length; i++)
            {
                targetMaterials[i] = new Material(targetMaterials[i]);
            }
            rend.materials = targetMaterials;
        }
        else
        {
            int totalMaterialCount = 0;
            foreach (var obj in paintableParts)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    totalMaterialCount += renderer.materials.Length;
                }
            }

            targetMaterials = new Material[totalMaterialCount];
            int index = 0;

            foreach (var obj in paintableParts)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer == null) continue;

                Material[] originalMats = renderer.materials;
                Material[] instanceMats = new Material[originalMats.Length];

                for (int i = 0; i < originalMats.Length; i++)
                {
                    instanceMats[i] = new Material(originalMats[i]);
                    targetMaterials[index++] = instanceMats[i];    
                }

                renderer.materials = instanceMats;
            }
        }
    }

    /// <summary>
    /// Creates and returns a list of colors that need to be detected by the player.
    /// Skips materials marked as exceptions. Also stores all correct colors.
    /// </summary>
    public List<NewColor> SetDetectedColorList()
    {
        List<NewColor> detectColorList = new List<NewColor>();
        for(int i = 0 ; i<targetMaterials.Length ; i++){
            rightColor.Add(targetMaterials[i].color);
            if(!exceptionIndex.Contains(i)){
                NewColor newColor = new NewColor(targetMaterials[i].color,false);
                detectColorList.Add(newColor);
            }
        }
        return detectColorList;
    }

    /// <summary>
    /// Sets all paintable materials to white except for exception indices.
    /// Used to reset the object for the coloring phase.
    /// </summary>
    public void SetAllWhite()
    {
        for(int i = 0 ; i<targetMaterials.Length ; i++){
            if(!exceptionIndex.Contains(i))
                SetColor(Color.white , targetMaterials[i]);
        }        
    }

    /// <summary>
    /// Sets a specific color to a given material.
    /// </summary>
    private void SetColor(Color color, Material mat)
    {
        mat.color = color;
    }

    /// <summary>
    /// Paints a specific material by index on a paintaleparts in model with the current color.
    /// </summary>
    public void Coloring(Color color, GameObject gameObject)
    {
        var go = gameObject.GetComponent<PaintablePart>();
        if(go!=null)
        {
            int index = go.GetMatIndex();
            SetColor(color,targetMaterials[index]);
        }
    }

    /// <summary>
    /// Shows the correct answer colors by restoring all materials to their original color.
    /// </summary>
    public void ShowCorrectColor()
    {
        for(int i=0 ; i<targetMaterials.Length ; i++)
        {
            targetMaterials[i].color = rightColor[i];
        }
    }

    /// <summary>
    /// Checks if all material colors match the stored correct colors.
    /// </summary>
    public bool CheckCorrect()
    {
        for(int i=0 ; i<targetMaterials.Length ; i++)
        {
            if(targetMaterials[i].color!=rightColor[i])
                return false;
        }
        return true;
    }
}
