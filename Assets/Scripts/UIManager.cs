using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject colorDetectUI;
    [SerializeField] private GameObject coloringUI;
    [SerializeField] private GameObject successUI;
    [SerializeField] private GameObject failUI;
    [SerializeField] private GameObject menuUI;

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
}
