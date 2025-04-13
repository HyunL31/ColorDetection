using UnityEngine;

public class ForTest : MonoBehaviour
{
    public void OnTouch()
    {
        UIManager.Instance.ReturnStart();
    }

    public void onLoad()
    {
        UIManager.Instance.Loading();
    }
}
