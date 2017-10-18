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
        lifetime = _lifetimeMax = seconds;
    }

    public void ReturnToPool()
    {
        StopAllParticleSystems();
        lifetime = _lifetimeMax;
        effectManager.ReturnToPool(this);
    }
}
