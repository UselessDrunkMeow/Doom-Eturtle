using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager _instance;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip PlayerMusic;
    [SerializeField] private AudioClip BossMusic;
    [SerializeField] private bool IsPlayingBossMusic = false;
    private AudioClip ClipToIncrease;

    /*public static AudioManager Instance
    {
        get
        {
            if (_instance == null) _instance = new AudioManager();
            return _instance;
        }
    }*/
    private void Awake()
    {
        if (_instance == null) _instance = this;
        else { Destroy(gameObject); return; }
        // DontDestroyOnLoad(gameObject);  // if it should survive scene loads
    }

    public static AudioManager Instance => _instance;   // replace the new AudioManager()
    public IEnumerator switchAudio()
    {
        if (IsPlayingBossMusic == true)
        {
            //ClipToLower = BossMusic;
            ClipToIncrease = PlayerMusic;
            IsPlayingBossMusic = false;
        }
        else
        {
            //ClipToLower = PlayerMusic;
            ClipToIncrease = BossMusic;
            IsPlayingBossMusic = true;
        }
        StartCoroutine(LowerVolume());
        yield return new WaitForSeconds(1f);
        StartCoroutine(IncreaseVolume(ClipToIncrease));
    }
    private IEnumerator LowerVolume()
    {
        Debug.Log("lowering audio");
        while (source.volume > 0f)
        {
            source.volume = Mathf.MoveTowards(source.volume, 1f, Time.deltaTime);
            if (source.volume > 0f)
            {
                source.Stop();
                Debug.Log("lowered audio");
                break;
            }
            yield return 0f;
        }
    }
    private IEnumerator IncreaseVolume(AudioClip clippie)
    {
        if (IsPlayingBossMusic == true)
        {
            source.volume = 1f;
            source.clip = clippie;
            source.Play();
            Debug.Log("Boss Music");
        }
        else
        {
            Debug.Log("increasing audio" + clippie);
            source.clip = clippie;
            source.Play();
            while (source.volume < 1f)
            {
                source.volume = Mathf.Lerp(source.volume, 0f, 1f * Time.deltaTime);
                yield return 0f;
            }
        }
    }
    private AudioManager()
    {
        //the constructor is private so that you can't instantiate it
    }
}
