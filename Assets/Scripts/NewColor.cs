using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NewColor : MonoBehaviour
{
    public Color answerColor {get; set;}
    public UnityEvent OnColorMaterial;
    private Color presentColor;
    public bool isDetected;

    //Generator
    public NewColor(Color color, bool isDet)
    {
        answerColor = color;
        isDetected = isDet;
    }

    public void PaintColor(){
        OnColorMaterial?.Invoke();
    }

    
}
