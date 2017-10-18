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

    private const string itemName = "[AUDIO] ";

    public int GetAudioIndex(string name)
    {
        for (int i = 0; i < _audioItemSettings.Length; i++)
        {
            if (_audioItemSettings[i].name == name)
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
    /// <param name="parent">Parent of the object. Null attachs the object to AudioManager.</param>
    /// <param name="pos">Position relative to parent.</param>
    public AudioSource GetAudio(string name, Transform parent = null, Vector3 pos = default(Vector3))
    {
        return GetAudio(GetAudioIndex(name), parent, pos);
    }

    /// <summary>
    /// Get GameObject and AudioSource with predefined settings.
    /// </summary>
    /// <param name="index">Index of the audio item in settings.</param>
    /// <param name="parent">Parent of the object. Null attachs the object to AudioManager.</param>
    /// <param name="pos">Position relative to parent.</param>
    public AudioSource GetAudio(int index, Transform parent = null, Vector3 pos = default(Vector3))
    {
        if (index <= 0 || index >= _audioItemSettings.Length)
            return null;
        AudioSource audio = new GameObject(itemName + _audioItemSettings[index].name, typeof(AudioSource)).GetComponent<AudioSource>();
        if (parent)
        {
            audio.transform.SetParent(parent);
        }
        else
        {
            audio.transform.SetParent(transform);
        }
        audio.playOnAwake = false;
        audio.transform.localPosition = pos;
        audio.clip = _audioItemSettings[index].clip;
        audio.outputAudioMixerGroup = _audioItemSettings[index].mixerGroup;
        audio.volume = _audioItemSettings[index].volume;
        audio.priority = _audioItemSettings[index].priority;
        audio.spatialBlend = _audioItemSettings[index].spatialBlend ? 0f : 1f;
        audio.loop = _audioItemSettings[index].loop;
        audio.mute = _audioItemSettings[index].mute;   
        return null;
    }
}
