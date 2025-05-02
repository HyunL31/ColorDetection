using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject Character;
    public Image CharacterImg;
    public Sprite HandUpBig;
    public Sprite HandUpSmall;
    public Sprite Hello;
    public Sprite HandDown;

    public TMP_Text Text1;
    public GameObject NextTextBtn;
    public GameObject StartGameBtn;
    private List<string> textList = new List<string>()
    {
        // 0
        "Hey, you there! Mind helping me out?",
        // 1
        "I want to make things colorful, but I haven't got any colors to paint with! Can you gather some for me?",
        // 2
        "Just use the camera to pick out some nice colors!",
        // 3
        "Then we can use your new colors to paint the objects! Choose a color and touch the part that you want to paint.",
        // 4
        "Thank you so much for helping me!",
    };

    private int currentText = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set button activation, character image and first dialogue text   
        StartGameBtn.SetActive(false);
        NextTextBtn.SetActive(true);
        CharacterImg = Character.GetComponent<Image>();
        Text1.text = textList[0];
        currentText += 1;
    }

    // Update is called once per frame
    void Update() { }

    public void ChangeDialogue()
    {
        // Change text
        Text1.text = textList[currentText];

        // Change character image according to dialogue
        if (currentText == 1)
        {
            CharacterImg.sprite = HandDown;
        }
        else if (currentText == 2)
        {
            CharacterImg.sprite = HandUpBig;
        }
        else if (currentText == 3)
        {
            CharacterImg.sprite = HandUpSmall;
        }
        else if (currentText == 4)
        {
            CharacterImg.sprite = HandDown;
            // Deactivate dialogue button and activate button to start the game
            NextTextBtn.SetActive(false);
            StartGameBtn.SetActive(true);
        }

        currentText += 1;
    }
}
