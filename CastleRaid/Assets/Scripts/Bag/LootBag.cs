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
    private GameObject[] bagMeshes;
    [SerializeField]
    Transform lootDestinationTransform;
	[SerializeField]
	bool initialState = false;
	List<Lootable> lootablesInLootTrigger = new List<Lootable>();
    int lootCount = -1;
    int lootTotalValue = -1;
    bool isActive = false;

    public delegate void IntVoid(int integer);
    public event IntVoid OnLootCountChange;
    public event IntVoid OnLootTotalValueChange;

	public delegate void BoolIntVoid(bool boolean, int integer);
	public event BoolIntVoid OnLootBagActiveStateChange;

	//TODO: Implement timer after ungrab to the Lootable script, to prevent looting all lootables by just waving the bag
	//Lootable would have to have been ungrabbed within half a second or so for the lootBag to accept it as a viable loot

	//TODO: Implement bag streching as it fills
	//TODO: Implement bag physics (joints)

	private void Start()
    {
		int count = bagMeshes.Length;
		for (int i = 0; i < count; i++)
		{
			isActive = true;
			SetActiveState(initialState, i);
		}

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
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
    }

    public void SetActiveState(bool newState, int bagMeshIndex = 0)
    {
		//Set whether the bag is active or not
		bool oldState = isActive;
        isActive = newState;

		if(oldState != isActive)
		{
			if (isActive)
			{
				lootAreaTrigger.enabled = true;

				if (bagMeshes[bagMeshIndex] != null)
				{
					bagMeshes[bagMeshIndex].SetActive(true);
				}
			}
			else
			{
				lootAreaTrigger.enabled = false;

				if (bagMeshes[bagMeshIndex] != null)
				{
					bagMeshes[bagMeshIndex].SetActive(false);
				}
			}

			OnLootBagActiveStateChange(isActive, bagMeshIndex);
		}
	}

	public void SetActiveState(bool newState, Transform newParent, int bagMeshIndex = 0)
	{
		SetParentTransform(newParent);
		SetActiveState(newState, bagMeshIndex);
	}

	private void OnTriggerEnter(Collider other)
    {
        if (isActive)
        {
            //If the entering object is a lootable
            Lootable lootable = other.GetComponent<Lootable>();
            if (lootable)
            {
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
        GameManager.effectManager.GetEffect("LootBurst", true, pos: transform.position);
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
