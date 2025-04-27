using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Tutorials.Core.Editor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public TMP_Text Text1;
    public GameObject BG;
    public GameObject BGDark;
    public GameObject NextTextBtn;
    public GameObject TutorialBtn;
    public Image BGImg;
    public Sprite IMG1;
    public Sprite IMG2;
    public Sprite IMG3;
    public Sprite IMG4;
    public Sprite IMG5;

    private List<string> textList = new List<string>()
    {
        // Phase 0
        "Once upon a time...",
        //Phase 1 - img1
        "In a village in the world of NOCOLOR, there was an old story passed down from the elders.",
        "It was a legend that warned of a curse that would blind anyone who entered the cave in the mountain behind the village. ",
        "Most people believed it was just a tale meant to scare children away from the dangerous cave.",
        //Phase 2 - img 2
        "However, one day, a boy ventured into the cave and returned with stories of a completely different world.",
        "That was when everyone realized that the cave was not just a dark, mysterious place—but a passage to a world full of color.",
        // img 3
        "What the villagers didn’t know was that, long ago, their world had been full of vibrant colors.",
        // img 4
        "But a mysterious force had stolen all of it, leaving everything in shades of gray.",
        //Phase 3 - img 5
        "The cave, as the boy discovered, was the only connection to the colorful world that had once been theirs.",
        "The villagers realized that the colors had been hidden in that world all along, waiting to be reclaimed.",
        //Phase 4
        "They will now go on an adventure to regain their color.",
    };

    private int currentText = 0;
    private int currentPhase = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set visibility of elements
        TutorialBtn.SetActive(false);
        NextTextBtn.SetActive(true);

        // Get background img and set first text
        BGImg = BG.GetComponent<Image>();
        Text1.text = textList[0];
        currentText += 1;
    }

    // Update is called once per frame
    void Update() { }

    public void ChangeDialogue()
    {
        // Change phase
        if (currentText == 1 || currentText == 3 || currentText == 7)
        {
            currentPhase += 1;
        }

        // Change text
        Text1.text = textList[currentText];

        // Change background image
        if (currentPhase == 1)
        {
            BGDark.GetComponent<Image>().CrossFadeAlpha(0.0f, 2.0f, false);
            BGImg.sprite = IMG1;
        }
        else if (currentPhase == 2)
        {
            BGDark.SetActive(false);
            if (currentText == 3)
            {
                BGImg.sprite = IMG2;
            }
            else if (currentText == 5)
            {
                BGImg.sprite = IMG3;
            }
            else if (currentText == 6)
            {
                BGImg.sprite = IMG4;
            }
        }
        else if (currentPhase == 3)
        {
            BGImg.sprite = IMG5;
            if (currentText == 10)
            {
                BGImg.CrossFadeAlpha(0.0f, 2.0f, false);
                NextTextBtn.SetActive(false);
                TutorialBtn.SetActive(true);
            }
        }

        currentText += 1;
    }

    public void StartTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
}
