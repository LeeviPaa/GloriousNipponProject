using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawnArea : MonoBehaviour
{
	[SerializeField]
	GameObject defaultLootableType;
	[SerializeField]
	List<Transform> lootSpawnTransforms = new List<Transform>();
	[SerializeField]
	List<LootSpawnInfo> lootSpawnInfos = new List<LootSpawnInfo>();

	private void Start()
	{
		int count = lootSpawnInfos.Count;
		for (int i = 0; i < count; i++)
		{
			GameObject lootableToSpawn = null;
			Transform spawnTransform = null;
			if (lootSpawnInfos[i].lootablePrefab != null)
			{
				lootableToSpawn = lootSpawnInfos[i].lootablePrefab;

				if(lootSpawnInfos[i].transformArrayIndex  < lootSpawnTransforms.Count)
				{
					spawnTransform = lootSpawnTransforms[lootSpawnInfos[i].transformArrayIndex];
				}
				else
				{
					Debug.LogWarning("transformArrayIndex out of lootSpawnTransforms bounds! Skipping lootable spawning");
				}
			}
			else if(defaultLootableType != null)
			{
				lootableToSpawn = defaultLootableType;
				spawnTransform = lootSpawnTransforms[i];
			}

			if (lootableToSpawn != null && spawnTransform != null)
			{
				Instantiate(lootableToSpawn, spawnTransform.position, spawnTransform.rotation);
			}
		}
	}


}

[System.Serializable]
public struct LootSpawnInfo
{
	public GameObject lootablePrefab;
	public int transformArrayIndex;
}
