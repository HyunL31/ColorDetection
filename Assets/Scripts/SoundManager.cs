using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //Sound Manager for Other script
    public static SoundManager _instance;
    public static SoundManager Instance { get { return _instance; } }
    [SerializeField] private AudioSource ac;
    [SerializeField] private AudioClip[] list;  //list of bgm and sfx source

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
            ac.Play();
        }
    }
    public void ControlVol(float vol)
    {
        ac.volume = vol;
    }

    //SFX for success
    public void Success()
    {
        GameObject sfxObject = new GameObject("SfxPlayer");
        AudioSource sfx = sfxObject.AddComponent<AudioSource>();
        sfx.clip = list[2];
        sfx.Play();
        Destroy(sfxObject, sfx.clip.length);
    }

    //SFX for fail
    public void Fail()
    {
        GameObject sfxObject = new GameObject("SfxPlayer");
        AudioSource sfx = sfxObject.AddComponent<AudioSource>();
        sfx.clip = list[3];
        sfx.Play();
        Destroy(sfxObject, sfx.clip.length);
    }

    //SFX for end
    public void End()
    {
        GameObject sfxObject = new GameObject("SfxPlayer");
        AudioSource sfx = sfxObject.AddComponent<AudioSource>();
        sfx.clip = list[4];
        sfx.Play();
        Destroy(sfxObject, sfx.clip.length);
    }
}
