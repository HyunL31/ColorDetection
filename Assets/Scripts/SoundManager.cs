using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    //Sound Manager for Other script
    public static SoundManager _instance;
    public static SoundManager Instance { get { return _instance; } }
    [SerializeField] private AudioSource ac;
    [SerializeField] private AudioClip[] list;  //list of bgm and sfx source
    enum mList { main, cutScene, tutorial, success, fail, congratulation };
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

    //BGM for main
    public void Main()
    {
        AudioClip clip = list[(int)mList.main];
        if (ac.isPlaying)
        {
            if (ac.clip != clip) StartCoroutine(FadeOutIn(clip));
        }
        else
        {
            ac.clip = clip;
            ac.Play();
        }
    }

    //BGM for CutScene
    public void CutScene()
    {
        AudioClip clip = list[(int)mList.cutScene];
        if (ac.isPlaying)
        {
            if (ac.clip != clip) StartCoroutine(FadeOutIn(clip));
        }
        else
        {
            ac.clip = clip;
            ac.Play();
        }
    }

    //BGM for Tutorial
    public void Tutorial()
    {
        AudioClip clip = list[(int)mList.tutorial];
        if (ac.isPlaying)
        {
            if (ac.clip != clip) StartCoroutine(FadeOutIn(clip));
        }
        else
        {
            ac.clip = clip;
            ac.Play();
        }
    }

    //SFX for success
    public void Success()
    {
        GameObject sfxObject = new GameObject("SfxPlayer");
        AudioSource sfx = sfxObject.AddComponent<AudioSource>();
        sfx.clip = list[(int)mList.success];
        sfx.Play();
        Destroy(sfxObject, sfx.clip.length);
    }

    //SFX for fail
    public void Fail()
    {
        GameObject sfxObject = new GameObject("SfxPlayer");
        AudioSource sfx = sfxObject.AddComponent<AudioSource>();
        sfx.clip = list[(int)mList.fail];
        sfx.Play();
        Destroy(sfxObject, sfx.clip.length);
    }

    //SFX for end
    public void End()
    {
        GameObject sfxObject = new GameObject("SfxPlayer");
        AudioSource sfx = sfxObject.AddComponent<AudioSource>();
        sfx.clip = list[(int)mList.congratulation];
        sfx.Play();
        Destroy(sfxObject, sfx.clip.length);
    }

    //Coroutine for fadeout and fadein
    private IEnumerator FadeOutIn(AudioClip clip)
    {
        float startVolume = ac.volume;
        float dur = 0.5f;

        while (ac.volume > 0)
        {
            ac.volume -= startVolume * Time.deltaTime / dur;
            yield return null;
        }
        ac.Stop();
        ac.volume = 0f;

        ac.clip = clip;
        ac.Play();

        while (ac.volume < startVolume)
        {
            ac.volume += Time.deltaTime / dur;
            yield return null;
        }

        ac.volume = startVolume;
    }
}
