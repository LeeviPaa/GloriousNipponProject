using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBag : MonoBehaviour
{
    //private AudioSource sound;
    //private UnityEngine.UI.Text txtBelongings;

    [SerializeField]
    private Collider lootAreaTrigger;
    [SerializeField]
    private GameObject bagMesh;
    [SerializeField]
    Transform lootDestinationTransform;
    List<Lootable> lootablesInLootTrigger = new List<Lootable>();
    int lootCount = -1;
    int lootTotalValue = -1;
    bool isActive = false;

    public delegate void IntVoid(int integer);
    public event IntVoid OnLootCountChange;
    public event IntVoid OnLootTotalValueChange;

    private void Start()
    {
        SetActiveState(true);

        lootCount = 0;
        lootTotalValue = 0;
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            //If there are any items in the list of lootables inside the looting trigger
            int count = lootablesInLootTrigger.Count;
            if (count > 0)
            {
                //Loop through all of them one by one
                for (int i = 0; i < count; i++)
                {
                    //If the object is not currently grabbed
                    if (!lootablesInLootTrigger[i].GetIsGrabbed())
                    {
                        //Loot it and remove it from the list
                        lootablesInLootTrigger[i].OnLootingFinished += OnLootableLooted;
                        lootablesInLootTrigger[i].Loot(lootDestinationTransform);
                        lootablesInLootTrigger.RemoveAt(i);
                    }
                }
            }
        }
    }

    public void SetParentTransform(Transform newParent)
    {
        //Set a new transform for the loot bag to follow
        transform.SetParent(newParent);
    }

    public void SetActiveState(bool newState)
    {
        //Set whether the bag is active or not
        isActive = newState;

        if (isActive)
        {
            lootAreaTrigger.enabled = true;

            if(bagMesh != null)
            {
                bagMesh.SetActive(true);
            }
        }
        else
        {
            lootAreaTrigger.enabled = false;

            if (bagMesh != null)
            {
                bagMesh.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("LootBag.OnTriggerEnter");

        if (isActive)
        {
            //If the entering object is a lootable
            Lootable lootable = other.GetComponent<Lootable>();
            if (lootable)
            {
                Debug.Log("LootBag.OnTriggerEnter, lootable component found on entering object");
                //If it is not currently grabbed
                if (!lootable.GetIsGrabbed())
                {
                    //Loot it
                    lootable.OnLootingFinished += OnLootableLooted;
                    lootable.Loot(lootDestinationTransform);
                }
                else
                {
                    //Else, add it to the list of lootables inside the trigger
                    lootablesInLootTrigger.Add(lootable);
                }
            }
        }

    }
    
    private void OnTriggerExit(Collider other)
    {
        if (isActive)
        {
            Lootable lootable = other.GetComponent<Lootable>();
            //If the exiting object is a lootable
            if (lootable)
            {
                //If it is contained in the list of lootables inside the trigger
                if (lootablesInLootTrigger.Contains(lootable))
                {
                    //Remove it from the list
                    lootablesInLootTrigger.Remove(lootable);
                }
            }
        }

    }

    private void OnLootableLooted(int lootValue)
    {
        lootCount++;
        if(OnLootCountChange != null)
        {
            OnLootCountChange(lootCount);
        }

        lootTotalValue += lootValue;
        if (OnLootTotalValueChange != null)
        {
            OnLootTotalValueChange(lootTotalValue);
        }

        //Call appropriate visual / sound effects here if the effects are the same regardless of the lootable
    }

    public bool GetActiveState()
    {
        return isActive;
    }

    public int GetLootCount()
    {
        return lootCount;
    }

    public int GetLootTotalValue()
    {
        return lootTotalValue;
    }

}
