using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBag : MonoBehaviour
{
    public enum EBagType
    {
        HAND,
        BACK,

    }

    [SerializeField]
    EBagType bagType;

    [SerializeField]
    private Collider[] lootAreaTriggers;
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

    [SerializeField]
    Vector3 rightHandOffset;
    [SerializeField]
    Vector3 leftHandOffset;

    public delegate void IntVoid(int integer);
    public event IntVoid OnLootCountChange;
    public event IntVoid OnLootTotalValueChange;

    public delegate void BoolIntVoid(bool boolean, int integer);
    public event BoolIntVoid OnLootBagActiveStateChange;

    //TODO: Implement bag streching as it fills
    //TODO: Implement bag physics (joints)

    private void Start()
    {
        if(bagType == EBagType.BACK)
        {
            int length = lootAreaTriggers.Length;
            for (int i = 0; i < length; i++)
            {
                ColliderController colliderController = lootAreaTriggers[i].GetComponent<ColliderController>();
                if(colliderController != null)
                {
                    colliderController._OnTriggerEnter += OnLootAreaTriggerEnter;
                }
                else
                {
                    Debug.LogError("lootAreaTrigger at array index " + i + " is missing a ColliderController component! " 
                        + "(Detecting separate trigger events requires the ColliderController script on the triggers)");
                }
            }



        }

        int count = bagMeshes.Length;
        for (int i = 0; i < count; i++)
        {
            isActive = true;
            SetActiveState(initialState, i);
        }

        lootCount = 0;
        lootTotalValue = 0;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.J))
    //    {
    //        OnLootableLooted(25);
    //    }
    //}

    //private void FixedUpdate()
    //{
    //    if (isActive)
    //    {
    //        //If there are any items in the list of lootables inside the looting trigger
    //        int count = lootablesInLootTrigger.Count;
    //        if (count > 0)
    //        {
    //            //Loop through all of them one by one
    //            for (int i = 0; i < count; i++)
    //            {
    //                //If the object is not currently grabbed
    //                if (!lootablesInLootTrigger[i].GetIsGrabbed())
    //                {
    //                    //Loot it and remove it from the list
    //                    LootLootable(lootablesInLootTrigger[i]);
    //                    lootablesInLootTrigger.RemoveAt(i);
    //                }
    //            }
    //        }
    //    }
    //}

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

        if (oldState != isActive)
        {
            if (isActive)
            {
                int count = lootAreaTriggers.Length;
                for (int i = 0; i < count; i++)
                {
                    lootAreaTriggers[i].enabled = true;
                }

                if (bagMeshes[bagMeshIndex] != null)
                {
                    bagMeshes[bagMeshIndex].SetActive(true);
                }
            }
            else
            {
                int count = lootAreaTriggers.Length;
                for (int i = 0; i < count; i++)
                {
                    lootAreaTriggers[i].enabled = false;
                }

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

        if (bagMeshIndex == 0)
        {
            transform.localPosition = rightHandOffset;
        }
        else if (bagMeshIndex == 1)
        {
            transform.localPosition = leftHandOffset;
        }

        SetActiveState(newState, bagMeshIndex);
    }

    private void LootLootable(Lootable lootable, bool playEffect = true)
    {
        //Subscribe to the LootingFinished event and loot the lootable
        lootable.OnLootingFinished += OnLootableLooted;
        lootable.Loot(lootDestinationTransform);

        if (playEffect)
        {
            //Call looting effect
            GameManager.effectManager.GetEffect("LootBurst", true, transform.position, transform.rotation, transform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActive)
        {
            //If the entering object is a lootable
            Lootable lootable = other.GetComponent<Lootable>();
            if (lootable)
            {
                switch (bagType)
                {
                    //If the bagType is HAND
                    case EBagType.HAND:
                        Debug.Log("OnTriggerEnter bagType == HAND, this is ok!");
                        LootLootable(lootable);
                        break;

                    //If the bagType is BACK
                    case EBagType.BACK:
                        Debug.Log("OnTriggerEnter bagType == BACK, this is NOT ok!");
                        if (lootable.GetIsGrabbed())
                        {
                            LootLootable(lootable);
                        }
                        break;

                    default:
                        break;
                }
            }
        }

    }

    private void OnLootAreaTriggerEnter(ColliderController colliderController, Collider other)
    {
        //If the entering object is a lootable
        Lootable lootable = other.GetComponent<Lootable>();
        if (lootable)
        {
            //If the bagType is BACK
            if (bagType == EBagType.BACK)
            {
                Debug.Log("OnLootAreaTriggerEnter");
                //If the lootable is currently grabbed
                if (lootable.GetIsGrabbed())
                {
                    LootLootable(lootable, false);

                    //Determine which side the lootable was looted on
                    Transform lootingEffectMarker = null;

                    if(colliderController != null)
                    {
                        lootingEffectMarker = colliderController.transform;
                    }

                    if (lootingEffectMarker == null)
                    {
                        Debug.LogError("bagType == BACK, but determining looting side failed! Check if this happens, and fix if deemed necessary");
                        lootingEffectMarker = transform;
                    }

                    //Spawn the LootBurst effect on the appropriate location
                    GameManager.effectManager.GetEffect("LootBurst", true, lootingEffectMarker.position, lootingEffectMarker.rotation, lootingEffectMarker);
                }
            }
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (isActive)
    //    {
    //        Lootable lootable = other.GetComponent<Lootable>();
    //        //If the exiting object is a lootable
    //        if (lootable)
    //        {
    //            //If it is contained in the list of lootables inside the trigger
    //            if (lootablesInLootTrigger.Contains(lootable))
    //            {
    //                //Remove it from the list
    //                lootablesInLootTrigger.Remove(lootable);
    //            }
    //        }
    //    }

    //}

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

        //Call appropriate visual / sound effects here if the effects are the same regardless of the lootable
        AudioItem lootingFinishedAudio = GameManager.audioManager.GetAudio("LootingFinished", true, pos: transform.position);
        lootingFinishedAudio.source.Play();

        //Debug.Log("OnLootableLooted");
        //EffectItem valueEffect = GameManager.effectManager.GetEffect("NumberDisplay", false, transform.position, transform.rotation);
        //valueEffect.GetComponent<CreateTexture>().ChangeShowValue(lootValue);
        //valueEffect.GetComponent<ParticleSystem>().Clear();
        //valueEffect.GetComponent<ParticleSystem>().Play();
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
