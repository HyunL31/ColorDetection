using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is the custom color class for checking the color is detected.
/// The colormanager generates the list of this class from prefab's colors.
/// After detect the color, if that color is similar with what we want to find, the colormanager changes
/// this class's 'isDetected' to true.
/// Finally, colormanager can check which colors are detected from checking this class's 'isDetected'.
/// </summary>
public class NewColor : MonoBehaviour
{
    public Color answerColor {get; set;}
    public bool isDetected;

    //Generator
    public NewColor(Color color, bool isDet)
    {
        answerColor = color;
        isDetected = isDet;
    }
}
