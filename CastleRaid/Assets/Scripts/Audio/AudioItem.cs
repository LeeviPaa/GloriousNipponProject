using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioItem : MonoBehaviour
{
    public AudioSource source;
    public AudioManager audioManager;
    public bool autoReturnAfterPlaying = false;

    private bool isPlaying = false;

    public virtual void Init()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
    }

    protected virtual void Update()
    {
        if (autoReturnAfterPlaying && isPlaying && !source.isPlaying)
        {
            ReturnToPool();
        }
        isPlaying = source.isPlaying;
    }

    public void ReturnToPool()
    {
        source.Stop();
        audioManager.ReturnToPool(this);
    }
}
