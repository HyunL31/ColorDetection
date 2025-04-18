using System;
using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //UI Manager for Other script
    public static UIManager _instance;
    public static UIManager Instance {  get { return _instance; } }
    public GameObject UI;
    public Slider sound;
    public Toggle[] colorType;

    int currentUI = 0;
    float currentSound = 0.4f;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else { 
            _instance = this;
            DontDestroyOnLoad(this);
            DontDestroyOnLoad(UI);
            SoundManager._instance.ControlVol(currentSound);
        }
    }

    //Go to settings UI
    public void GoSetting()
    {
        UiTouch(0, 1);
    }

    //Go to Loading UI
    public void Loading()
    {
        UiTouch(1, 0);
        UI.transform.GetChild(0).gameObject.SetActive(false);
        UI.transform.GetChild(1).gameObject.SetActive(true);
        currentUI = -1;
        Time.timeScale = 1;
        StartCoroutine(OnLoad());
    }

    //Loading UI timer
    IEnumerator OnLoad()
    {
        yield return new WaitForSeconds(0.3f);
        UI.transform.GetChild(0).gameObject.SetActive(true);
        UI.transform.GetChild(1).gameObject.SetActive(false);
        OutUI();
    }

    //End the game
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    //Start the game
    public void StartGame()
    {
        //this needs name of Next Scene.
        //Assign Test Scene 
        UI.GetNamedChild("Main").SetActive(false);
        OutUI();
        SceneManager.LoadScene("Test Scene");
    }

    //Return to the main
    public void ReturnStart()
    {
        UiTouch(0, 0);
    }


    //Adjust Volume
    public void ScaleVolume()
    {
        currentSound = sound.value;
        SoundManager._instance.ControlVol(currentSound);
    }

    //Switch color type between RGB and Hexadecimal
    //0 is RGB(default), 1 is Hexadecimal
    public void ColorCheck(int type)
    {
        colorType[1-type].isOn = colorType[type].isOn == true ? false : true;

        //Notify current color type
    }

    //Load Type can be 0 and 1
    //UI name = {Main, Loading, Settings}
    private void UiTouch(int which, int index, int loadType = 0)
    {
        GameObject target = UI.transform.GetChild(0).gameObject;
        InUI(which);
        if(currentUI >= 0) target.transform.GetChild(currentUI).gameObject.SetActive(false);
        currentUI = index;
        target.transform.GetChild(index).gameObject.SetActive(true);

    }

    //UI on/off
    //pIndex is which Panel. 0 = default, 1 = translucent
    private void InUI(int pIndex)
    {
        Time.timeScale = 0;

        if (!UI.activeSelf)
        {
            UI.SetActive(true);
        }
        
        if (pIndex == 0 && !UI.transform.GetChild(pIndex).gameObject.activeSelf)
        {
            UI.transform.GetChild(0).gameObject.SetActive(true);
            UI.transform.GetChild(1).gameObject.SetActive(false);
        } else if (pIndex == 1 && !UI.transform.GetChild(pIndex).gameObject.activeSelf)
        {
            UI.transform.GetChild(1).gameObject.SetActive(true);
            UI.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    private void OutUI()
    {
        UI.SetActive(false);
        Time.timeScale = 1f;
    }
}
