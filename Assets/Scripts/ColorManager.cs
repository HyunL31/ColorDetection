using UnityEngine;
using System;
/// <summary>
/// Have solution color list and check that.
/// So this Script have compare color method and check all answers are foounded.
/// If it works, this script execute event about that.
/// </summary>
public class ColorManager : MonoBehaviour
{
    [SerializeField] private ColorDetector colorDetector;
    public event EventHandler ColorDetectEvent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
