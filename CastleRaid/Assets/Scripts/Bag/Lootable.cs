using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Lootable : MonoBehaviour
{
	VRTK_InteractableObject interactableObjectScript;
	bool isGrabbed = false;
	bool isLooted = false;
	[SerializeField]
	int lootValue = 100;

	public delegate void IntVoid(int integer);
	public event IntVoid OnLootingFinished;

	bool minimizingToBag = false;
	Vector3 minimizingStartScale = Vector3.zero;
	Vector3 minimizingEndScale = Vector3.zero;
	Vector3 minimizingStartPos = Vector3.zero;
	float minimizingStartTime = -1f;
	float minimizingDuration = 0.5f;

	//TODO: Implement timer after ungrab to the Lootable script, to prevent looting all lootables by just waving the bag
	//Lootable would have to have been ungrabbed within half a second or so for the lootBag to accept it as a viable loot

	private void Start()
	{
		minimizingToBag = false;
	}

	void OnEnable()
	{
		interactableObjectScript = GetComponent<VRTK_InteractableObject>();

		if (interactableObjectScript)
		{
			interactableObjectScript.InteractableObjectGrabbed -= OnStartGrab;
			interactableObjectScript.InteractableObjectGrabbed += OnStartGrab;
			interactableObjectScript.InteractableObjectUngrabbed -= OnEndGrab;
			interactableObjectScript.InteractableObjectUngrabbed += OnEndGrab;
		}

		//Call passive indicator effect
		GameManager.effectManager.GetEffect("GlitterEffect", true, Vector3.zero, Quaternion.identity, transform);
	}
	void OnDisable()
	{
		if (interactableObjectScript)
		{
			interactableObjectScript.InteractableObjectGrabbed -= OnStartGrab;
			interactableObjectScript.InteractableObjectUngrabbed -= OnEndGrab;
		}

		if (minimizingToBag)
		{
			FinishLooting();
		}
	}

	void OnStartGrab(object sender, InteractableObjectEventArgs e)
	{
		isGrabbed = true;

		//Call grab visual / sound effects here
		GameManager.effectManager.GetEffect("GrabBurst", true, transform.position, Quaternion.identity);
		//GameManager.audioManager.GetAudio("LootGrab", pos: transform.position);
	}

	void OnEndGrab(object sender, InteractableObjectEventArgs e)
	{
		isGrabbed = false;
	}

	public bool GetIsGrabbed()
	{
		return isGrabbed;
	}

	public int GetLootValue()
	{
		return lootValue;
	}

	public void Loot(Transform lootingPoint)
	{
		//Set the lootable state to is looted
		isLooted = true;

		//Start the loot "animation"
		minimizingToBag = true;
		transform.SetParent(lootingPoint);
		minimizingStartPos = transform.localPosition;
		minimizingStartScale = transform.localScale;
		minimizingStartTime = Time.time;
	}

	private void MinimizeToLootBag()
	{
		float timeSinceStarted = Time.time - minimizingStartTime;
		float percentageCompleted = timeSinceStarted / minimizingDuration;

		transform.localScale = Vector3.Lerp(minimizingStartScale, minimizingEndScale, percentageCompleted);
		transform.localPosition = Vector3.Lerp(minimizingStartPos, Vector3.zero, percentageCompleted);

		if (percentageCompleted >= 1)
		{
			minimizingToBag = false;
			FinishLooting();
		}
	}

	private void FinishLooting()
	{
		if (OnLootingFinished != null)
		{
			OnLootingFinished(lootValue);
		}

		OnLootingFinished = null;
		//Call the visual / sound effects here, if unique to each lootable

		gameObject.SetActive(false);
	}

	private void FixedUpdate()
	{
		if (minimizingToBag)
		{
			MinimizeToLootBag();
		}
	}

}
