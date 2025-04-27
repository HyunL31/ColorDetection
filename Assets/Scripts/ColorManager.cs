using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.XR.CoreUtils;

/// <summary>
/// Have solution color list and check that.
/// So this Script have compare color method and check all answers are foounded.
/// If it works, this script execute event about that.
/// </summary>
public class ColorManager : MonoBehaviour
{
    [Header("Color Detector")]
    [SerializeField] private ColorDetector colorDetector;
    public float colorTolerance = 0.2f;

    [Header("Goal Color Ui")]
    [SerializeField] private GameObject targetColorUI;

    [Header("Coloring Resource UI")]
    [SerializeField] private GameObject coloringResUI;

    [Header("Event")]
    public UnityEvent OnEndColorDetect;

    private AnswerColorList answerColorList;
    private List<NewColor> haveToDetect;
    private Color presentColor;
    private int numOfDetect = 0;
    private int numHaveToDetect = 0;

    public void SetAnswerColorList(GameObject targetObject)
    {
        answerColorList = targetObject.GetComponent<AnswerColorList>();
        answerColorList.SetMaterials();
    }

    public void SetWhite()
    {
        answerColorList.SetAllWhite();
    }

    public void SetHaveToDetectList()
    {
        haveToDetect = answerColorList.SetDetectedColorList();
        if(haveToDetect ==null)
            Debug.Log("haveToDetect is null");
        else{
            numHaveToDetect = haveToDetect.Count;
        }
    }

    public void DetectColor()
    {
        NewColor detectedColor = colorDetector.DetectColorOnDemand();
        int index = CompareColor(detectedColor);
        if(index!=-1){
            numOfDetect++;
            MoveToColoringPart(index);
        }
        if(numOfDetect==haveToDetect.Count){
            OnEndColorDetect.Invoke();
        }
    }

    private int CompareColor(NewColor newColor){
        for(int i = 0 ; i<numHaveToDetect ; i++)
        {
            if(!haveToDetect[i].isDetected)
            {
                float rDiff = Mathf.Abs(haveToDetect[i].answerColor.r - newColor.answerColor.r);
                float gDiff = Mathf.Abs(haveToDetect[i].answerColor.g - newColor.answerColor.g);
                float bDiff = Mathf.Abs(haveToDetect[i].answerColor.b - newColor.answerColor.b);

                if (rDiff <= colorTolerance && gDiff <= colorTolerance && bDiff <= colorTolerance)
                {
                    haveToDetect[i].isDetected = true;
                    return i;
                }
            }
        }
        return -1;
    }

    public void MakeTargetColorUI()
    {
        for(int i = 0 ; i<numHaveToDetect ; i++)
        {
            targetColorUI.transform.GetChild(i).GetComponent<Image>().color
            = haveToDetect[i].answerColor;
            targetColorUI.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void MoveToColoringPart(int index)
    {
        targetColorUI.transform.GetChild(index).GetComponent<Image>().color
        = Color.white;
        targetColorUI.transform.GetChild(index).gameObject.SetActive(false);
    }

    public void MakeColoringUI()
    {
        for(int i = 0 ; i<numHaveToDetect ; i++)
        {
            coloringResUI.transform.GetChild(i).GetComponent<Image>().color
            = haveToDetect[i].answerColor;
            coloringResUI.transform.GetChild(i).gameObject.SetActive(true);
        }
        //reset
        presentColor = Color.white;
    }

    public void Paint(GameObject touchPart)
    {
        Debug.Log("painting!");
        answerColorList.Coloring(presentColor, touchPart);
    }

    public void SetPresentColor(Image image)
    {
        presentColor = image.color;
    }

    public bool CheckCorrected()
    {
        return answerColorList.CheckCorrect();
    }

    public void ShowCorrect()
    {
        answerColorList.ShowCorrectColor();
    }

    public void ResetNumberOfDetect()
    {
        numOfDetect = 0;
    }
}
