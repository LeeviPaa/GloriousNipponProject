using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class HandAnimationController : MonoBehaviour
{
	public enum EHandSide
	{
		RIGHT,
		LEFT,

	}

	[SerializeField]
	EHandSide handSide;
	[SerializeField]
	LootBag lootBag;

	Animator handAnimator;

	GameObject controllerObject;
	VRTK_ControllerEvents controllerEvents;
	bool controllerReferenceFound = false;

	bool holdingLootBag;

	GameObject vrtk_grabber;

	private void OnEnable()
	{
		handAnimator = GetComponent<Animator>();
		lootBag.OnLootBagActiveStateChange += OnLootBagActiveStateChange;

		controllerReferenceFound = false;
	}

	private void OnDisable()
	{
		lootBag.OnLootBagActiveStateChange -= OnLootBagActiveStateChange;

		if (controllerEvents)
		{
			controllerEvents.GripPressed -= OnGripPressed;
			controllerEvents.GripReleased -= OnGripReleased;
		}
	}

	private void Update()
	{
		if (!controllerReferenceFound)
		{
			FindControllerReferencesIfNotYetFound();

			if (controllerReferenceFound)
			{
				controllerEvents.GripPressed -= OnGripPressed;
				controllerEvents.GripPressed += OnGripPressed;
				controllerEvents.GripReleased -= OnGripReleased;
				controllerEvents.GripReleased += OnGripReleased;
			}
		}
	}

	void FindControllerReferencesIfNotYetFound()
	{
		if (controllerObject == null)
		{
			if (handSide == EHandSide.RIGHT)
			{
				controllerObject = VRTK_DeviceFinder.GetControllerRightHand(false);
			}
			else if (handSide == EHandSide.LEFT)
			{
				controllerObject = VRTK_DeviceFinder.GetControllerLeftHand(false);
			}
		}

		if (controllerObject != null)
		{
			controllerEvents = controllerObject.GetComponent<VRTK_ControllerEvents>();
		}

		if (controllerEvents)
		{
			controllerReferenceFound = true;
		}
	}

	private void OnGripPressed(object sender, ControllerInteractionEventArgs e)
	{
		if (!holdingLootBag)
		{
			//Close hand
			handAnimator.Play("Hand_Closed");
		}
	}

	private void OnGripReleased(object sender, ControllerInteractionEventArgs e)
	{
		if (!holdingLootBag)
		{
			//Open hand
			handAnimator.Play("Hand_Idle");
		}
	}

	private void OnLootBagActiveStateChange(bool bagActive, int bagMesh)
	{
		if (bagMesh == (int)handSide)
		{
			if (bagActive)
			{
				holdingLootBag = true;

				//Close hand
				handAnimator.Play("Hand_Closed");
				
				if (controllerObject)
				{
					VRTK_InteractGrab grab = controllerObject.GetComponent<VRTK_InteractGrab>();
					if (grab)
					{
						grab.enabled = false;
					}
					VRTK_InteractTouch touch = controllerObject.GetComponent<VRTK_InteractTouch>();
					if (touch)
					{
						touch.enabled = false;
					}
				}
			}
			else
			{
				holdingLootBag = false;

				//Close hand
				handAnimator.Play("Hand_Idle");

				if (controllerObject)
				{
					VRTK_InteractGrab grab = controllerObject.GetComponent<VRTK_InteractGrab>();
					if (grab)
					{
						grab.enabled = true;
					}
					VRTK_InteractTouch touch = controllerObject.GetComponent<VRTK_InteractTouch>();
					if (touch)
					{
						touch.enabled = true;
					}
				}
			}
		}
	}

}
