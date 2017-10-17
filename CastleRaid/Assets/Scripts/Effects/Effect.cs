using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    [HideInInspector]
    public EffectManager effectManager;
    [HideInInspector]
    public int poolIndex;

    [SerializeField]
    protected bool lifetimeActive = false;
    [SerializeField]
    protected float lifetime = 0f;

    protected ParticleSystem[] particleSystems;

    protected virtual void Awake()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    protected virtual void Update()
    {
        if (lifetimeActive)
            lifetime -= Time.deltaTime;
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
        lifetimeActive = true;
        lifetime = seconds;
    }
}
