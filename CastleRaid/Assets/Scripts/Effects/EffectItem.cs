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
	public float lifetime = 0f;

	protected float _lifetimeMax = 0f;

	protected ParticleSystem[] _particleSystems;

	public virtual void Init()
	{
		_particleSystems = GetComponentsInChildren<ParticleSystem>();
		_lifetimeMax = lifetime;
	}

	public virtual void ResetEffect()
	{
		lifetime = _lifetimeMax;
	}

    protected virtual void Update()
    {
        if (lifetimeActive)
        {
            lifetime -= Time.deltaTime;
            if (lifetime <= 0f)
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

    public virtual void SetLifetime(float seconds)
    {
        lifetimeActive = true;
        lifetime = seconds;
    }

    public void ReturnToPool()
    {
        StopAllParticleSystems();
        effectManager.ReturnToPool(this);
    }
}
