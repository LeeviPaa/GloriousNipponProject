using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LootBag))]
public class LootBagDisplayController : MonoBehaviour
{
    enum EDisplayType
    {
        LOOTCOUNT,
        TOTALVALUE,
    }

    [SerializeField]
    EDisplayType displayType;
    [SerializeField]
    TextMesh textMesh; //Change to other than TextMesh if necessary in the future
    LootBag lootBag;

    private void OnEnable()
    {
        if (textMesh == null)
        {
            Debug.LogError("Missing text mesh reference!");
        }
        else
        {
            textMesh.text = 0.ToString();
        }

        lootBag = GetComponent<LootBag>();

        lootBag.OnLootCountChange += OnLootCountChange;
        lootBag.OnLootTotalValueChange += OnLootTotalValueChange;
    }

    private void OnDisable()
    {
        lootBag.OnLootCountChange -= OnLootCountChange;
        lootBag.OnLootTotalValueChange -= OnLootTotalValueChange;
    }

    private void OnLootCountChange(int newLootCount)
    {
        if (displayType == EDisplayType.LOOTCOUNT)
        {
            textMesh.text = newLootCount.ToString();
        }
    }

    private void OnLootTotalValueChange(int newLootTotalValue)
    {
        if (displayType == EDisplayType.TOTALVALUE)
        {
            textMesh.text = newLootTotalValue.ToString();
        }
    }

}
