using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    private AudioSource _seSource;
    private AudioSource _bgmSource;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (_instance == null)
        {
            GameObject go = new GameObject("GlobalAudioManager");
            _instance = go.AddComponent<AudioManager>();
            
            _instance._seSource = go.AddComponent<AudioSource>();
            _instance._bgmSource = go.AddComponent<AudioSource>();
            
            DontDestroyOnLoad(go);
        }
    }

    public static void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (_instance != null && clip != null)
        {
            _instance._seSource.PlayOneShot(clip, volume);
        }
    }

    public static void PlayBGM(AudioClip clip, bool loop = true, float volume = 0.5f)
    {
        if (_instance != null && clip != null)
        {
            if (_instance._bgmSource.clip == clip && _instance._bgmSource.isPlaying) return;
            
            _instance._bgmSource.clip = clip;
            _instance._bgmSource.loop = loop;
            _instance._bgmSource.volume = volume;
            _instance._bgmSource.Play();
        }
    }

    public static void StopBGM()
    {
        if (_instance != null)
        {
            _instance._bgmSource.Stop();
        }
    }
}
