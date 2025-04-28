using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnswerColorList : MonoBehaviour
{
    [SerializeField] private List<PaintablePart> paintableParts;
    [SerializeField] private GameObject mainGO;
    [SerializeField] private int[] exceptionIndex;
    public Material[] targetMaterials;
    public List<Color> rightColor;
    private Renderer rend;

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

    public void SetAllWhite()
    {
        for(int i = 0 ; i<targetMaterials.Length ; i++){
            if(!exceptionIndex.Contains(i))
                SetColor(Color.white , targetMaterials[i]);
        }        
    }

    private void SetColor(Color color, Material mat)
    {
        mat.color = color;
    }

    public void Coloring(Color color, GameObject gameObject)
    {
        var go = gameObject.GetComponent<PaintablePart>();
        if(go!=null)
        {
            int index = go.GetMatIndex();
            SetColor(color,targetMaterials[index]);
        }
    }

    public void ShowCorrectColor()
    {
        for(int i=0 ; i<targetMaterials.Length ; i++)
        {
            targetMaterials[i].color = rightColor[i];
        }
    }

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
