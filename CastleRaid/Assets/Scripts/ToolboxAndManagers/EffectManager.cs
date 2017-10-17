using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectPoolSettings
{
    public string name;
    public GameObject prefab;
    public int poolSize;
    [HideInInspector]
    public Transform parent;
}

public class EffectManager : MonoBehaviour
{
    [SerializeField]
    private EffectPoolSettings[] effectPoolSettings;
    private List<Effect>[] effectPool;

    void Awake()
    {
        effectPool = new List<Effect>[effectPoolSettings.Length];
        for (int i = 0; i < effectPoolSettings.Length; i++)
        {
            effectPool[i] = new List<Effect>();

            for (int p = 0; p < effectPoolSettings[i].poolSize; p++)
            {
                CreateEffectPoolItem(i);
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

    public Effect GetEffect(string name)
    {
        return GetEffect(GetEffectIndex(name));
    }

    public Effect GetEffect(int index)
    {
        for (int i = 0; i < effectPool[index].Count; i++)
        {
            if (!effectPool[index][i].gameObject.activeSelf)
            {
                effectPool[index][i].gameObject.SetActive(true);
                return effectPool[index][i];
            }
        }
        Effect newEffect = CreateEffectPoolItem(index);
        newEffect.gameObject.SetActive(true);
        return newEffect;
    }

    public void ReturnToPool(Effect self)
    {
        if (effectPoolSettings.Length <= self.poolIndex)
            return;
        self.gameObject.SetActive(false);
        self.transform.SetParent(effectPoolSettings[self.poolIndex].parent);
    }

    private Effect CreateEffectPoolItem(int index)
    {
        if (!effectPoolSettings[index].parent)
            CreateEffectPoolParent(index);
        Effect effect = Instantiate(effectPoolSettings[index].prefab, Vector3.zero, Quaternion.identity, effectPoolSettings[index].parent).GetComponent<Effect>();
        effect.effectManager = this;
        effect.poolIndex = index;
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
