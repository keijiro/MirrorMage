using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    private AudioSource _seSource;
    private AudioSource _reverbSeSource;
    private AudioSource _bgmSource;

    [Header("Mixer Settings")]
    private AudioMixerGroup bypassGroup;
    private AudioMixerGroup reverbGroup;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (_instance == null)
        {
            GameObject go = new GameObject("GlobalAudioManager");
            _instance = go.AddComponent<AudioManager>();
            
            _instance._seSource = go.AddComponent<AudioSource>();
            _instance._reverbSeSource = go.AddComponent<AudioSource>();
            _instance._bgmSource = go.AddComponent<AudioSource>();
            
            // Load Mixer Groups
            AudioMixer mixer = Resources.Load<AudioMixer>("MainMixer");
            if (mixer == null)
            {
                // Try to find it in the project (only works in editor or if assigned, but for now we search)
    #if UNITY_EDITOR
                mixer = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioMixer>("Assets/Audio/MainMixer.mixer");
    #endif
            }

            if (mixer != null)
            {
                AudioMixerGroup[] bypassGroups = mixer.FindMatchingGroups("Bypass");
                if (bypassGroups.Length > 0) _instance.bypassGroup = bypassGroups[0];

                AudioMixerGroup[] reverbGroups = mixer.FindMatchingGroups("Reverb");
                if (reverbGroups.Length > 0) _instance.reverbGroup = reverbGroups[0];
            }
            
            DontDestroyOnLoad(go);
        }
    }

    private void Update()
    {
        // Apply groups if assigned
        if (_seSource != null && bypassGroup != null) _seSource.outputAudioMixerGroup = bypassGroup;
        if (_reverbSeSource != null && reverbGroup != null) _reverbSeSource.outputAudioMixerGroup = reverbGroup;
        if (_bgmSource != null && bypassGroup != null) _bgmSource.outputAudioMixerGroup = bypassGroup;
    }

    public static void PlaySFX(AudioClip clip, float volume = 1f, bool useReverb = false)
    {
        if (_instance != null && clip != null)
        {
            if (useReverb)
            {
                _instance._reverbSeSource.PlayOneShot(clip, volume);
            }
            else
            {
                _instance._seSource.PlayOneShot(clip, volume);
            }
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
