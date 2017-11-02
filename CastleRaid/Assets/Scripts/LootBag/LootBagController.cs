using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBagController : MonoBehaviour
{
	[SerializeField]
	LootBag primaryLootBag;
	[SerializeField]
	LootBag secondaryLootBag;

	int lootCount = -1;
	int lootTotalValue = -1;

	public delegate void IntVoid(int integer);
	public event IntVoid OnLootCountChange;
	public event IntVoid OnLootTotalValueChange;

    public const string lootTotalValueSaveKey = "TotalLootValue";

    private void Start()
	{
		lootCount = 0;
		lootTotalValue = 0;
        LevelInstance_Game li = GameManager.levelInstance as LevelInstance_Game;
        if (li)
        {
            li.levelTimeEnded += OnLevelTimeEnd;
        }
    }

	private void OnEnable()
	{
		if (primaryLootBag)
		{
			//primaryLootBag.SetLootBagController(this);
			primaryLootBag.OnLootableLooted += OnLootableLooted;
		}

		if (secondaryLootBag)
		{
			//secondaryLootBag.SetLootBagController(this);
			secondaryLootBag.OnLootableLooted += OnLootableLooted;
		}
	}

	private void OnDisable()
	{
		if (primaryLootBag)
		{
			primaryLootBag.OnLootableLooted -= OnLootableLooted;
		}

		if (secondaryLootBag)
		{
			secondaryLootBag.OnLootableLooted -= OnLootableLooted;
		}
	}

	private void OnLootableLooted(int lootValue)
	{
		lootCount++;
		if (OnLootCountChange != null)
		{
			OnLootCountChange(lootCount);
		}

		lootTotalValue += lootValue;
		if (OnLootTotalValueChange != null)
		{
			OnLootTotalValueChange(lootTotalValue);
		}
	}

	public int GetLootCount()
	{
		return lootCount;
	}

	public int GetLootTotalValue()
	{
		return lootTotalValue;
	}

    void OnLevelTimeEnd(object sender, LevelInstance_Game.LevelEventArgs e)
    {
        DataStorage.StoreData(lootTotalValueSaveKey, lootTotalValue);
    }
}
