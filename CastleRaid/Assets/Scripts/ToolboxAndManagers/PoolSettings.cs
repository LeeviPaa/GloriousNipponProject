using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolSettings
{
    public string name;
    public GameObject prefab;
    public int poolSize;
    [HideInInspector]
    public Transform parent;
	public float lifetime;
}
