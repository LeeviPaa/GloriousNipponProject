using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectItem : MonoBehaviour
{
    [HideInInspector]
    public EffectManager effectManager;
    [HideInInspector]
    public int poolIndex;

    public bool _lifetimeActive = false;
    public float _lifetime = 0f;

    protected ParticleSystem[] particleSystems;

    public virtual void Init()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    protected virtual void Update()
    {
        if (_lifetimeActive)
        {
            _lifetime -= Time.deltaTime;
            if (_lifetime <= 0f)
            {
                ReturnToPool();
            }
        }
    }

    public virtual void PlayAllParticleSystems()
    {
        foreach (ParticleSystem sys in particleSystems)
        {
            sys.Play();
        }
    }

    public virtual void PauseAllParticleSystems()
    {
        foreach (ParticleSystem sys in particleSystems)
        {
            sys.Pause();
        }
    }

    public virtual void StopAllParticleSystems()
    {
        foreach (ParticleSystem sys in particleSystems)
        {
            sys.Stop();
        }
    }

    public virtual void SetLifetime(float seconds)
    {
        _lifetimeActive = true;
        _lifetime = seconds;
    }

    public void ReturnToPool()
    {
        StopAllParticleSystems();
        effectManager.ReturnToPool(this);
    }
}
