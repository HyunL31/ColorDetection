using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnswerColorList : MonoBehaviour
{
    [SerializeField] private List<NewColor> detectColorList;
    [SerializeField] private List<GameObject> paintableObjects;
    [SerializeField] private GameObject mainGO;
    [SerializeField] int exceptionIndex;
    private Material[] targetMaterials;
    private Renderer rend;
    public float colorTolerance;

    public void SetMaterials()
    {
        if(mainGO!=null)
        {
            Debug.Log("works");
            rend = mainGO.GetComponent<Renderer>();
            targetMaterials = rend.materials;
            for (int i = 0; i < targetMaterials.Length; i++)
            {
                if(i != exceptionIndex )
                    targetMaterials[i] = new Material(targetMaterials[i]); 
            }
            rend.materials = targetMaterials;
        }
    }

    public List<NewColor> GetAnswerColorList()
    {
        return detectColorList;
    }

    public void SetDetectedColorList()
    {
        foreach(var mat in targetMaterials){
            NewColor newColor = new NewColor(mat.color,false);
            detectColorList.Add(newColor);
        }
    }

    public void SetAllWhite(){
        Debug.Log("set white");
        foreach(Material mat in targetMaterials)
        {
            mat.color = Color.white;
        }
    }

//move to colormanager
/*
    private void CompareColorList(Color detectedColor)
    {
        foreach(NewColor newColor in detectColorList)
        {
            if(!newColor.isDetected)
            {
                float rDiff = Mathf.Abs(detectedColor.r - newColor.answerColor.r);
                float gDiff = Mathf.Abs(detectedColor.g - newColor.answerColor.g);
                float bDiff = Mathf.Abs(detectedColor.b - newColor.answerColor.b);

                if (rDiff <= colorTolerance && gDiff <= colorTolerance && bDiff <= colorTolerance)
                {

                }
            }
        }
    }
    */

}
