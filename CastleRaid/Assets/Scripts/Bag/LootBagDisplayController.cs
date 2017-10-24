using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

[RequireComponent(typeof(LootBag))]
public class LootBagDisplayController : MonoBehaviour
{
    enum EDisplayType
    {
        LOOTCOUNT,
        TOTALVALUE,
    }

	[SerializeField]
	Vector3 rightHandOffset;
	[SerializeField]
	Vector3 leftHandOffset;

    [SerializeField]
    EDisplayType displayType;
	[SerializeField]
	GameObject displayHolder;
	[SerializeField]
    TextMesh textMesh; //Change to other than TextMesh if necessary in the future
    LootBag lootBag;

	[SerializeField]
	bool floating = false;
	Transform mainCameraTransform;
	//Vector3 floatingOffset;
	bool headsetReferenceFound = false;

    private void OnEnable()
    {
		if (floating)
		{
			//floatingOffset = transform.localPosition;
			displayHolder.transform.SetParent(null);
			displayHolder.transform.rotation = Quaternion.identity;

			headsetReferenceFound = false;
			mainCameraTransform = VRTK_DeviceFinder.HeadsetCamera();

			if (mainCameraTransform != null)
			{
				headsetReferenceFound = true;
			}
		}

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
		lootBag.OnLootBagActiveStateChange += OnLootBagActiveStateChange;

	}

    private void OnDisable()
    {
        lootBag.OnLootCountChange -= OnLootCountChange;
        lootBag.OnLootTotalValueChange -= OnLootTotalValueChange;
    }

	private void LateUpdate()
	{
		if (floating)
		{
			displayHolder.transform.position = lootBag.transform.position/* + floatingOffset*/;

			if (headsetReferenceFound)
			{
				displayHolder.transform.LookAt(mainCameraTransform);
			}
			else
			{
				mainCameraTransform = VRTK_DeviceFinder.HeadsetCamera();

				if (mainCameraTransform != null)
				{
					headsetReferenceFound = true;
				}
			}
		}
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

	private void OnLootBagActiveStateChange(bool newState, int bagMeshIndex)
	{
		if(displayHolder != null)
		{
			displayHolder.SetActive(newState);
		}
	}

}
