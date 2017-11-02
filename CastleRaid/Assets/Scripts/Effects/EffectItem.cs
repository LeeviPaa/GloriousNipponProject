using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectItem : MonoBehaviour
{
	[HideInInspector]
	public EffectManager effectManager;
	[HideInInspector]
	public int poolIndex;

	public bool lifetimeActive = false;
	public float currentLifetime = 0f;
	public float lifetime = 0f;

	protected ParticleSystem[] _particleSystems;

	public virtual void Init()
	{
		_particleSystems = GetComponentsInChildren<ParticleSystem>();
		currentLifetime = lifetime;
	}

    protected virtual void Update()
    {
        if (lifetimeActive)
        {
            currentLifetime -= Time.deltaTime;
            if (currentLifetime <= 0f)
            {
                ReturnToPool();
            }
        }
    }

    public virtual void PlayAllParticleSystems()
    {
        foreach (ParticleSystem sys in _particleSystems)
        {
            sys.Play();
        }
    }

    public virtual void PauseAllParticleSystems()
    {
        foreach (ParticleSystem sys in _particleSystems)
        {
            sys.Pause();
        }
    }

    public virtual void StopAllParticleSystems()
	{
		foreach (ParticleSystem sys in _particleSystems)
        {
            sys.Stop();
        }
    }

    public virtual void Reset()
    {
        currentLifetime = lifetime;
		StopAllParticleSystems();
	}

    public void ReturnToPool()
    {
		Reset();
        effectManager.ReturnToPool(this);
    }
}
