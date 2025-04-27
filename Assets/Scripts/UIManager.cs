using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private static UIManager _instance;
    [SerializeField] private static UIManager Instance {  get { return _instance; } }
    [SerializeField] private GameObject UI;
    [SerializeField] private Slider sound;
    [SerializeField] private Toggle[] colorType;

    int currentUI = 0;
    float currentSound = 0.4f;

    [SerializeField] private GameObject colorDetectUI;
    [SerializeField] private GameObject coloringUI;
    [SerializeField] private GameObject successUI;
    [SerializeField] private GameObject failUI;
    [SerializeField] private GameObject menuUI;
    public UnityEvent OnRestart;

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

    //Return to the main
    public void ReturnStart()
    {
        UiTouch(0, 0);
        Time.timeScale = 1;
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

    public void SetColorDetectUI(bool targetState){
        if(colorDetectUI != null)
            colorDetectUI.SetActive(targetState);
    }

    public void SetColoringUI(bool targetState){
        if(coloringUI != null)
            coloringUI.SetActive(targetState);
    }

    public void SetSuccssUI(bool targetState)
    {
        if(successUI!=null)
            successUI.SetActive(targetState);
    }

    public void SetFailUI(bool targetState)
    {
        if(failUI!=null)
            failUI.SetActive(targetState);
    }

    public void SetColoringButton(bool right)
    {
        GameObject submitBut = coloringUI.transform.GetChild(0).gameObject;
        GameObject retryBut = coloringUI.transform.GetChild(1).gameObject;
        submitBut.SetActive(right);
        retryBut.SetActive(!right);
    }

    public bool IsOverUI(Vector2 pos)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(pos.x,pos.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Exists(r => r.gameObject.layer == LayerMask.NameToLayer("UI"));
    }
}
