using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.XR.CoreUtils;

/// <summary>
/// Manages all color-related logic in the game, including:
/// - Receiving answer colors from the model
/// - Detecting user-captured colors and validating them
/// - Managing color-related UI for detection and coloring
/// - Handling painting and correctness check
/// </summary>
public class ColorManager : MonoBehaviour
{
    [Header("Color Detector")]
    // Reference to the component responsible for color detection
    [SerializeField] private ColorDetector colorDetector;
    public float colorTolerance = 0.2f; // Tolerance for determining color similarity

    [Header("Goal Color Ui")]
    // UI for displaying target colors during detection phase
    [SerializeField] private GameObject targetColorUI; 

    [Header("Coloring Resource UI")]
    // UI for displaying selectable colors during coloring phase
    [SerializeField] private GameObject coloringResUI;

    // Image to display the currently selected color
    [SerializeField] private Image currentColor;

    [Header("Event")]
    // Event triggered when all required colors are successfully detected
    public UnityEvent OnEndColorDetect;

    // Holds the original answer color and materials information from the model
    private AnswerColorList answerColorList;
    private List<NewColor> haveToDetect; // List of colors that need to be detected
    private Color presentColor; // Currently selected color to paint with
    private int numOfDetect = 0; // Number of successfully detected colors
    private int numHaveToDetect = 0; // Total number of colors to detect

    /// <summary>
    /// Get the answer color list from the provided model.
    /// </summary>
    public void SetAnswerColorList(GameObject targetObject)
    {
        answerColorList = targetObject.GetComponent<AnswerColorList>();
        answerColorList.SetMaterials();
    }

    /// <summary>
    /// Sets all object materials to white (reset state).
    /// </summary>
    public void SetWhite()
    {
        answerColorList.SetAllWhite();
    }

    /// <summary>
    /// Stores the list of colors the player must detect.
    /// </summary>
    public void SetHaveToDetectList()
    {
        haveToDetect = answerColorList.SetDetectedColorList();
        if(haveToDetect ==null)
            Debug.Log("haveToDetect is null");
        else{
            numHaveToDetect = haveToDetect.Count;
        }
    }

    /// <summary>
    /// This method works when user press detect button at detection phase.
    /// Detects a color and compares it with the haveToDetect list.
    /// If matched, marks it as found and updates UI.
    /// </summary>
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

    /// <summary>
    /// Compares the detected color with remaining colors in the haveToDetect list.
    /// Returns the index of a matched color, or -1 if none match.
    /// </summary>
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

    /// <summary>
    /// Activates and sets color for UI elements that show target colors.
    /// Called during the detection phase.
    /// </summary>
    public void MakeTargetColorUI()
    {
        for(int i = 0 ; i<numHaveToDetect ; i++)
        {
            targetColorUI.transform.GetChild(i).GetComponent<Image>().color
            = haveToDetect[i].answerColor;
            targetColorUI.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Hides a color from the detection UI when it's detected.
    /// </summary>
    private void MoveToColoringPart(int index)
    {
        targetColorUI.transform.GetChild(index).GetComponent<Image>().color
        = Color.white;
        targetColorUI.transform.GetChild(index).gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets up the palette UI with detected colors for coloring phase.
    /// </summary>
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

    /// <summary>
    /// Paints the selected part of the object with the currently selected color.
    /// </summary>
    public void Paint(GameObject touchPart)
    {
        Debug.Log("painting!");
        answerColorList.Coloring(presentColor, touchPart);
    }

    /// <summary>
    /// Sets the currently selected paint color and updates the UI.
    /// </summary>
    /// <param name="image"></param>
    public void SetPresentColor(Image image)
    {
        presentColor = image.color;
        SetCurrentUI(presentColor);
    }
    
    /// <summary>
    /// Updates the color shown in the current color UI image.
    /// </summary>
    /// <param name="color"></param>
    private void SetCurrentUI(Color color)
    {
        if(currentColor!=null)
            currentColor.color = color;
    }

    /// <summary>
    /// Checks whether all parts were painted with correct colors.
    /// </summary>
    public bool CheckCorrected()
    {
        return answerColorList.CheckCorrect();
    }

    /// <summary>
    /// Reveals the correct coloring on the object (e.g. after failure).
    /// </summary>
    public void ShowCorrect()
    {
        answerColorList.ShowCorrectColor();
    }

    /// <summary>
    /// Resets internal state and color UI for a new game round and going to main menu.
    /// </summary>
    public void ResetColorManager()
    {
        numOfDetect = 0;
        SetCurrentUI(Color.white);
        ResetChildObject(targetColorUI);
        ResetChildObject(coloringResUI);
    }

    /// <summary>
    /// Resets all children of a given UI parent (color and visibility).
    /// </summary>
    private void ResetChildObject(GameObject parent)
    {
        List<GameObject> gos = new List<GameObject>();
        parent.GetChildGameObjects(gos);
        if(gos!=null)
        {
            foreach(GameObject go in gos)
            {
                go.GetComponent<Image>().color = Color.white;
                go.SetActive(false);
            }
        }
    }
}
