using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioItemSettings
{
    public string name;
    public AudioClip clip;
    public AudioMixerGroup mixerGroup;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(256, 0)]
    public int priority = 128;
    public bool spatialBlend;
    public bool loop = false;
    public bool mute = false;
}

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioItemSettings[] _audioItemSettings;
    [SerializeField]
    private int _poolSize;
    private List<AudioItem> _audioItemPool = new List<AudioItem>();

    private const string itemName = "[AUDIO]";

    void Awake()
    {
        GameManager.audioManager = this;

        for (int i = 0; i < _poolSize; i++)
        {
            AddAudioItem();
        }
    }

    public int GetAudioIndex(string name)
    {
        name = name.ToLower().Trim();
        for (int i = 0; i < _audioItemSettings.Length; i++)
        {
            if (_audioItemSettings[i].name.ToLower().Trim() == name)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Get GameObject and AudioSource with predefined settings.
    /// </summary>
    /// <param name="name">Name of the audio item in settings.</param>
    /// <param name="oneShot">Automatically return to pool after playing once.</param>
    /// <param name="parent">Parent of the object. Null attachs the object to AudioManager.</param>
    /// <param name="pos">World position.</param>
    public AudioItem GetAudio(string name, bool play, bool oneShot, Vector3 pos, Transform parent = null)
    {
        return GetAudio(GetAudioIndex(name), play, oneShot, pos, parent);
    }

    /// <summary>
    /// Get GameObject and AudioSource with predefined settings.
    /// </summary>
    /// <param name="index">Index of the audio item in settings.</param>
    /// <param name="oneShot">Automatically return to pool after playing once.</param>
    /// <param name="parent">Parent of the object. Null attachs the object to AudioManager.</param>
    /// <param name="pos">World position.</param>
    public AudioItem GetAudio(int index, bool play, bool oneShot, Vector3 pos, Transform parent = null)
    {
        if (index < 0 || index >= _audioItemSettings.Length)
            return null;
        AudioItem audio = null;
        for (int i = 0; i < _audioItemPool.Count; i++)
        {
            if (!_audioItemPool[i].gameObject.activeSelf)
            {
                audio = _audioItemPool[i];
                break;
            }
        }
        if (!audio)
        {
            audio = AddAudioItem();
        }
        if (parent)
        {
            audio.transform.SetParent(parent);
        }
        audio.gameObject.SetActive(true);
        audio.transform.position = pos;
        audio.source.clip = _audioItemSettings[index].clip;
        audio.source.outputAudioMixerGroup = _audioItemSettings[index].mixerGroup;
        audio.source.volume = _audioItemSettings[index].volume;
        audio.source.priority = _audioItemSettings[index].priority;
        audio.source.spatialBlend = _audioItemSettings[index].spatialBlend ? 0f : 1f;
        audio.source.loop = _audioItemSettings[index].loop;
        audio.source.mute = _audioItemSettings[index].mute;
        audio.autoReturnAfterPlaying = oneShot;
        audio.name = itemName + " " + _audioItemSettings[index].name;
        if (play)
        {
            audio.source.Play();
        }
        return audio;
    }

    private AudioItem AddAudioItem()
    {
        AudioItem audio = new GameObject(itemName, typeof(AudioSource), typeof(AudioItem)).GetComponent<AudioItem>();
        _audioItemPool.Add(audio);       
        audio.audioManager = this;
        audio.Init();
        ReturnToPool(audio);
        return audio;
    }

    public void ReturnToPool(AudioItem self)
    {
        self.gameObject.SetActive(false);
        self.transform.SetParent(transform);
        self.transform.localPosition = Vector3.zero;
        self.name = itemName;
    }
}
