using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public struct AudioData
    {
        public AudioID id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
        public bool useReverb;
    }

    [Header("Clips")]
    [SerializeField] private AudioData[] audioDataList = default;

    [Header("Mixer Settings")]
    [SerializeField] private AudioMixerGroup bypassGroup = default;
    [SerializeField] private AudioMixerGroup reverbGroup = default;

    private Dictionary<AudioID, AudioClip> _clipDict = new Dictionary<AudioID, AudioClip>();
    private Dictionary<AudioID, float> _volumeDict = new Dictionary<AudioID, float>();
    private Dictionary<AudioID, bool> _reverbDict = new Dictionary<AudioID, bool>();

    private AudioSource _seSource;
    private AudioSource _reverbSeSource;
    private AudioSource _bgmSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Initialize();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        _clipDict.Clear();
        _volumeDict.Clear();
        _reverbDict.Clear();

        foreach (var data in audioDataList)
        {
            if (data.id == AudioID.None) continue;
            _clipDict[data.id] = data.clip;
            _volumeDict[data.id] = data.volume;
            _reverbDict[data.id] = data.useReverb;
        }

        _seSource = gameObject.AddComponent<AudioSource>();
        _seSource.outputAudioMixerGroup = bypassGroup;

        _reverbSeSource = gameObject.AddComponent<AudioSource>();
        _reverbSeSource.outputAudioMixerGroup = reverbGroup;

        _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.outputAudioMixerGroup = bypassGroup;
    }

    public static void PlaySFX(AudioID id)
    {
        if (Instance == null) return;
        
        if (Instance._clipDict.TryGetValue(id, out var clip))
        {
            float volume = Instance._volumeDict.ContainsKey(id) ? Instance._volumeDict[id] : 1f;
            bool useReverb = Instance._reverbDict.ContainsKey(id) && Instance._reverbDict[id];
            
            if (useReverb)
            {
                Instance._reverbSeSource.PlayOneShot(clip, volume);
            }
            else
            {
                Instance._seSource.PlayOneShot(clip, volume);
            }
        }
    }

    public static void PlayBGM(AudioID id, bool loop = true)
    {
        if (Instance == null) return;

        if (Instance._clipDict.TryGetValue(id, out var clip))
        {
            if (Instance._bgmSource.clip == clip && Instance._bgmSource.isPlaying) return;
            
            float volume = Instance._volumeDict.ContainsKey(id) ? Instance._volumeDict[id] : 0.5f;

            Instance._bgmSource.clip = clip;
            Instance._bgmSource.loop = loop;
            Instance._bgmSource.volume = volume;
            Instance._bgmSource.Play();
        }
    }

    public static void StopBGM()
    {
        if (Instance != null)
        {
            Instance._bgmSource.Stop();
        }
    }
}
