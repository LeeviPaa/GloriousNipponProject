using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField]
    private PoolSettings[] effectPoolSettings;
    private List<EffectItem>[] effectPool;

    void Awake()
    {
        GameManager.effectManager = this;

        effectPool = new List<EffectItem>[effectPoolSettings.Length];
        for (int i = 0; i < effectPoolSettings.Length; i++)
        {
            effectPool[i] = new List<EffectItem>();

            for (int p = 0; p < effectPoolSettings[i].poolSize; p++)
            {
                AddEffectPoolItem(i);
            }
        }
    }

    public int GetEffectIndex(string name)
    {
        name = name.ToLower().Trim();
        for (int i = 0; i < effectPoolSettings.Length; i++)
        {
            if (effectPoolSettings[i].name.ToLower().Trim() == name)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Get predefined Effect object from pool at specified location
    /// </summary>
    /// <param name="name">Name of the effect in settings.</param>
    /// <param name="play">Autoplay.</param>
    /// <param name="lifetime">Duration before automatically returned to pool.</param>
    /// <param name="pos">World position.</param>
    /// <param name="rot">World rotation.</param>
    /// <param name="parent">Parent of the object. Null attachs the object to EffectManager.</param>
    public EffectItem GetEffect(string name, bool play, bool oneShot, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        return GetEffect(GetEffectIndex(name), play, oneShot, pos, rot, parent);
    }

    /// <summary>
    /// Get predefined Effect object from pool at specified location
    /// </summary>
    /// <param name="index">Index of the effect in settings.</param>
    /// <param name="play">Autoplay.</param>
    /// <param name="pos">World position.</param>
    /// <param name="rot">World rotation.</param>
    /// <param name="parent">Parent of the object. Null leaves the object to pool location.</param>
    public EffectItem GetEffect(int index, bool play, bool oneShot, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        if (index < 0 || index >= effectPool.Length)
		{
			return null;
		}
        EffectItem effect = null;
		for (int i = 0; i < effectPool[index].Count; i++)
        {
            if (!effectPool[index][i].gameObject.activeSelf)
            {
                effect = effectPool[index][i];
                break;
            }
        }
        if (!effect)
        {
            effect = AddEffectPoolItem(index);
        }       
        if (parent)
        {
            effect.transform.SetParent(parent);
        }
        effect.transform.position = pos;
        effect.transform.rotation = rot;
        effect.gameObject.SetActive(true);
		if (play)
		{
			effect.PlayAllParticleSystems();
		}
		if (oneShot)
		{
			effect.lifetimeActive = true;
		}
		else
		{
			effect.lifetimeActive = false;
		}
		return effect;
    }

    public void ReturnToPool(EffectItem self)
    {
        if (effectPoolSettings.Length <= self.poolIndex)
            return;
        self.gameObject.SetActive(false);
        self.transform.SetParent(effectPoolSettings[self.poolIndex].parent);
        self.transform.localPosition = Vector3.zero;
        self.transform.localRotation = Quaternion.identity;
    }

    private EffectItem AddEffectPoolItem(int index)
    {
        if (!effectPoolSettings[index].parent)
            CreateEffectPoolParent(index);
        EffectItem effect = Instantiate(effectPoolSettings[index].prefab, Vector3.zero, Quaternion.identity, effectPoolSettings[index].parent).GetComponent<EffectItem>();
        effect.gameObject.SetActive(false);
        effect.effectManager = this;
        effect.poolIndex = index;
        effect.name = effectPoolSettings[index].name + "[" + effectPool[index].Count + "]";
		effect.currentLifetime = effectPoolSettings[index].lifetime;
		effect.Init(); 
        effectPool[index].Add(effect);
        return effect;
    }

    private Transform CreateEffectPoolParent(int index)
    {
        if (effectPoolSettings[index].parent)
            Destroy(effectPoolSettings[index].parent.gameObject);
        Transform parent = new GameObject("[POOL] " + effectPoolSettings[index].name).transform;
        parent.SetParent(transform);
        effectPoolSettings[index].parent = parent;
        return parent;
    }
}
