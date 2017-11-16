using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

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

    Queue<Transform> lootBurstSpawnPoints = new Queue<Transform>();
	Transform transformToFollow;

    int activeState = -1;
    bool isActive = false;
	bool backTransformInitialized = false;

	[SerializeField]
	Vector3 rightHandOffset;
	[SerializeField]
	Vector3 leftHandOffset;

    bool initialized = false;

	public delegate void IntVoid(int integer);
	public event IntVoid OnLootableLooted;

	public delegate void BoolIntVoid(bool boolean, int integer);
	public event BoolIntVoid OnLootBagActiveStateChange;

    //[SerializeField]
    private string itemLootedSoundSmall = "LootedSmall";
    //[SerializeField]
    private int itemLootedSoundMediumThreshold = 1000;
    //[SerializeField]
    private string itemLootedSoundMedium = "LootedMedium";
    //[SerializeField]
    private int itemLootedSoundLargeThreshold = 5000;
    //[SerializeField]
    private string itemLootedSoundLarge = "LootedLarge";

    //TODO: Implement bag streching as it fills
    //TODO: Implement bag physics (joints)

    //public void SetLootBagController(LootBagController _lootBagController)
    //{
    //	lootBagController = _lootBagController;
    //}

    private void Start()
	{
        backTransformInitialized = false;

		if (bagType == EBagType.BACK)
		{
			int length = lootAreaTriggers.Length;
			for (int i = 0; i < length; i++)
			{
				ColliderController colliderController = lootAreaTriggers[i].GetComponent<ColliderController>();
				if (colliderController != null)
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
			isActive = !initialState;
			SetActiveState(initialState, i);
		}

        initialized = true;
    }

	private void Update()
	{
		if (bagType == EBagType.BACK && !backTransformInitialized)
		{
			Transform vrCameraTransform = VRTK_DeviceFinder.HeadsetCamera();

			if (vrCameraTransform != null)
			{
				transformToFollow = vrCameraTransform;
				backTransformInitialized = true;
			}
		}
	}

	private void FixedUpdate()
	{
		if (bagType == EBagType.BACK && backTransformInitialized)
		{
			transform.position = transformToFollow.position;
			Vector3 finalRotation = transformToFollow.eulerAngles;
			finalRotation.x = 0;
			finalRotation.z = 0;
			transform.rotation = Quaternion.Euler(finalRotation);
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
		if (!newState)
		{
			activeState = -1;
		}
		else
		{
			activeState = bagMeshIndex;
		}

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

				if (bagMeshIndex < bagMeshes.Length)
				{
					if (bagMeshes[bagMeshIndex] != null)
					{
						bagMeshes[bagMeshIndex].SetActive(true);
					}
                }

                if (initialized)
                {
                    //Call bag activation sound effect
                    AudioItem bagActivationSoundEffect = GameManager.audioManager.GetAudio("BagActivation", true, true, pos: transform.position);
                }
            }
			else
			{
				int count = lootAreaTriggers.Length;
				for (int i = 0; i < count; i++)
				{
					lootAreaTriggers[i].enabled = false;
				}

				int length = bagMeshes.Length;
				for (int i = 0; i < length; i++)
				{
					bagMeshes[i].SetActive(false);
				}
			}

			if (OnLootBagActiveStateChange != null)
			{
				OnLootBagActiveStateChange(isActive, bagMeshIndex);
			}
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
		lootable.OnLootingFinished -= OnLootingFinished;
		lootable.OnLootingFinished += OnLootingFinished;
		lootable.Loot(lootDestinationTransform);

        //if (playEffect)
        //{
        //    //Call looting effect
        //    GameManager.effectManager.GetEffect("LootBurst", true, transform.position, transform.rotation, transform);
        //}
    }

	private void OnTriggerEnter(Collider other)
	{
		if (isActive)
		{
			//If the entering object is a lootable
			Lootable lootable = other.GetComponent<Lootable>();
			if (lootable)
			{
				if (!lootable.GetIsLooted())
				{
					switch (bagType)
					{
						//If the bagType is HAND
						case EBagType.HAND:
							//Debug.Log("OnTriggerEnter bagType == HAND, this is ok!");
							LootLootable(lootable);
							break;

						//If the bagType is BACK
						case EBagType.BACK:
							//Debug.Log("OnTriggerEnter bagType == BACK, this is NOT ok!");
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

	}

	private void OnLootAreaTriggerEnter(ColliderController colliderController, Collider other)
	{
		//If the entering object is a lootable
		Lootable lootable = other.GetComponent<Lootable>();
		if (lootable)
		{
			if (!lootable.GetIsLooted())
			{
				//If the bagType is BACK
				if (bagType == EBagType.BACK)
				{
					//If the lootable is currently grabbed
					if (lootable.GetIsGrabbed())
					{
						LootLootable(lootable, false);

						//Determine which side the lootable was looted on
						Transform lootingEffectMarker = null;

						if (colliderController != null)
						{
							lootingEffectMarker = colliderController.transform;
						}

						if (lootingEffectMarker == null)
						{
							Debug.LogError("bagType == BACK, but determining looting side failed! Check if this happens, and fix if deemed necessary");
							lootingEffectMarker = transform;
						}

                        //Spawn the LootBurst effect on the appropriate location
                        //GameManager.effectManager.GetEffect("LootBurst", true, lootingEffectMarker.position, lootingEffectMarker.rotation, lootingEffectMarker);

                        lootBurstSpawnPoints.Enqueue(lootingEffectMarker);
                    }
				}
			}
		}
	}

	private void OnLootingFinished(int lootValue)
	{
		if (OnLootableLooted != null)
		{
			OnLootableLooted(lootValue);
        }

        //Call looting effect
        Transform effectSpawnTransform = transform;

        if (lootBurstSpawnPoints.Count > 0)
        {
             effectSpawnTransform = lootBurstSpawnPoints.Dequeue();
        }
		
        GameManager.effectManager.GetEffect("LootBurst", true, true, effectSpawnTransform.position, effectSpawnTransform.rotation, effectSpawnTransform);

        //Call appropriate visual / sound effects here if the effects are the same regardless of the lootable
        if (lootValue < itemLootedSoundMediumThreshold)
        {
            GameManager.audioManager.GetAudio(itemLootedSoundSmall, true, true, pos: transform.position);
        }
        if (lootValue >= itemLootedSoundMediumThreshold && lootValue < itemLootedSoundLargeThreshold)
        {
            GameManager.audioManager.GetAudio(itemLootedSoundMedium, true, true, pos: transform.position);
        }
        else
        {
            GameManager.audioManager.GetAudio(itemLootedSoundLarge, true, true, pos: transform.position);
        }

		EffectItem lootTextEffect = null;
		if (bagType == EBagType.HAND)
		{
			lootTextEffect = GameManager.effectManager.GetEffect("FadingFloatingText", true, true, effectSpawnTransform.position + Vector3.up * 0.1f, Quaternion.identity/*, effectSpawnTransform*/);
		}
		else if (bagType == EBagType.BACK)
		{
			//transform.position + transform.forward * 1f + transform.up * -0.5f
			lootTextEffect = GameManager.effectManager.GetEffect("FadingFloatingText", true, true, effectSpawnTransform.position + effectSpawnTransform.up * 0.5f, effectSpawnTransform.rotation/*, effectSpawnTransform*/);
		}

		if(lootTextEffect != null) 
		{
			FadingFloatingTextController textController = lootTextEffect.GetComponentInChildren<FadingFloatingTextController>();
			textController.InitializeText("+" + lootValue.ToString());
		}

	}

	public int GetActiveState()
	{
		return activeState;
	}

}
