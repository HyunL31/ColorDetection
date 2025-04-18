using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //Sound Manager for Other script
    public static SoundManager _instance;
    public static SoundManager Instance { get { return _instance; } }
    public AudioSource ac;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this);
            ac = GetComponent<AudioSource>();
            ac.Play();
        }
    }
    public void ControlVol(float vol)
    {
        ac.volume = vol;
        Debug.Log("now " + vol);
    }

    //can be added SFX.
    //In that case, ac should be chaned to array.
}
