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
        for (int i = 0; i < effectPoolSettings.Length; i++)
        {
            if (effectPoolSettings[i].name == name)
            {
                return i;
            }
        }
        return -1;
    }

	/// <summary>
	/// Get predefined Effect object.
	/// </summary>
	/// <param name="name">Name of the effect in settings.</param>
	/// <param name="play">Autoplay.</param>
	/// <param name="parent">Parent of the object. Null attachs the object to EffectManager.</param>
	/// <param name="pos">Position relative to parent.</param>
	public EffectItem GetEffect(string name, bool play, Transform parent = null, Vector3 pos = default(Vector3))
    {
        return GetEffect(GetEffectIndex(name), play, parent, pos);
    }

	/// <summary>
	/// Get predefined Effect object.
	/// </summary>
	/// <param name="name">Name of the effect in settings.</param>
	/// <param name="play">Autoplay.</param>
	/// <param name="lifetime">Duration before automatically returned to pool.</param>
	/// <param name="parent">Parent of the object. Null attachs the object to EffectManager.</param>
	/// <param name="pos">Position relative to parent.</param>
	public EffectItem GetEffect(string name, bool play, float lifetime, Transform parent = null, Vector3 pos = default(Vector3))
    {
        return GetEffect(name, play, lifetime, parent, pos);
    }

	/// <summary>
	/// Get predefined Effect object.
	/// </summary>
	/// <param name="index">Index of the effect in settings.</param>
	/// <param name="play">Autoplay.</param>
	/// <param name="lifetime">Duration before automatically returned to pool.</param>
	/// <param name="parent">Parent of the object. Null attachs the object to EffectManager.</param>
	/// <param name="pos">Position relative to parent.</param>
	public EffectItem GetEffect(int index, bool play, float lifetime, Transform parent = null, Vector3 pos = default(Vector3))
    {
        EffectItem item = GetEffect(index, play, parent, pos);
        item.SetLifetime(lifetime);
        return item;
    }

	/// <summary>
	/// Get predefined Effect object.
	/// </summary>
	/// <param name="index">Index of the effect in settings.</param>
	/// <param name="play">Autoplay.</param>
	/// <param name="parent">Parent of the object. Null attachs the object to EffectManager.</param>
	/// <param name="pos">Position relative to parent.</param>
	public EffectItem GetEffect(int index, bool play, Transform parent = null, Vector3 pos = default(Vector3))
    {
		if (index < 0 || index >= effectPool.Length)
		{
			return null;
		}
		for (int i = 0; i < effectPool[index].Count; i++)
        {
            if (!effectPool[index][i].gameObject.activeSelf)
            {
				if (!parent)
					parent = transform;
				effectPool[index][i].gameObject.SetActive(true);
                effectPool[index][i].transform.SetParent(parent);
                effectPool[index][i].transform.localPosition = pos;
				effectPool[index][i].PlayAllParticleSystems();
				return effectPool[index][i];
            }
        }
        EffectItem newEffect = AddEffectPoolItem(index);
        newEffect.gameObject.SetActive(true);
		newEffect.transform.SetParent(parent);
		newEffect.transform.localPosition = pos;
		newEffect.PlayAllParticleSystems();
		return newEffect;
    }

    public void ReturnToPool(EffectItem self)
    {
        if (effectPoolSettings.Length <= self.poolIndex)
            return;
        self.gameObject.SetActive(false);
        self.transform.SetParent(effectPoolSettings[self.poolIndex].parent);
    }

    private EffectItem AddEffectPoolItem(int index)
    {
        if (!effectPoolSettings[index].parent)
            CreateEffectPoolParent(index);
        EffectItem effect = Instantiate(effectPoolSettings[index].prefab, Vector3.zero, Quaternion.identity, effectPoolSettings[index].parent).GetComponent<EffectItem>();
        effect.effectManager = this;
        effect.poolIndex = index;
        effect.name = effectPoolSettings[index].name + "[" + effectPool[index].Count + "]";
        effect.Init();
        effect.gameObject.SetActive(false);
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
