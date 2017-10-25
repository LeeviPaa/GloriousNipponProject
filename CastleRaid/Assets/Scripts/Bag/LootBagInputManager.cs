using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

[RequireComponent(typeof(LootBag))]
public class LootBagInputManager : MonoBehaviour
{
	LootBag lootBag;

	GameObject rightController;
	GameObject leftController;
	VRTK_ControllerEvents controllerEventsRight;
	VRTK_ControllerEvents controllerEventsLeft;
	bool controllerReferencesFound = false;

	float bagActivateTriggerThreshold = 0.5f;
	bool rightTriggerReleased = false;
	bool leftTriggerReleased = false;

	private void OnEnable()
	{
		lootBag = GetComponent<LootBag>();

		controllerReferencesFound = false;

		rightTriggerReleased = true;
		leftTriggerReleased = true;
	}

	private void OnDisable()
	{
		if (controllerEventsRight)
		{
			controllerEventsRight.TriggerAxisChanged -= OnRightTriggerAxisChanged;
		}

		if (controllerEventsLeft)
		{
			controllerEventsLeft.TriggerAxisChanged -= OnLeftTriggerAxisChanged;
		}
	}

	private void Update()
	{
		//if (Input.GetKeyDown(KeyCode.L))
		//{
		//	lootBag.SetActiveState(!lootBag.GetActiveState(), VRTK_DeviceFinder.GetControllerRightHand(false).transform, 0);
		//}

		if (!controllerReferencesFound)
		{
			FindControllerReferencesIfNotYetFound();

			if (controllerReferencesFound)
			{
				controllerEventsRight.TriggerAxisChanged -= OnRightTriggerAxisChanged;
				controllerEventsRight.TriggerAxisChanged += OnRightTriggerAxisChanged;
				controllerEventsLeft.TriggerAxisChanged -= OnLeftTriggerAxisChanged;
				controllerEventsLeft.TriggerAxisChanged += OnLeftTriggerAxisChanged;
			}
		}
	}

	private void OnRightTriggerAxisChanged(object sender, ControllerInteractionEventArgs e)
	{
		if (e.buttonPressure >= bagActivateTriggerThreshold)
		{
			if (rightTriggerReleased)
			{
				rightTriggerReleased = false;
				//If the bag is already on the right hand
				if (lootBag.transform.parent == rightController.transform)
				{
					//Toggle on current hand
					if (lootBag.GetActiveState() >= 0)
					{
						lootBag.SetActiveState(false, 0);
					}
					else
					{
						lootBag.SetActiveState(true, 0);
					}
				}
				else
				{
					//Change hands
					lootBag.SetActiveState(false, 1);
					lootBag.SetActiveState(true, rightController.transform, 0);
				}
			}
		}
		else if(!rightTriggerReleased)
		{
			rightTriggerReleased = true;
		}
	}

	private void OnLeftTriggerAxisChanged(object sender, ControllerInteractionEventArgs e)
	{
		if (e.buttonPressure >= bagActivateTriggerThreshold)
		{
			if (leftTriggerReleased)
			{
				leftTriggerReleased = false;
				//If the bag is already on the left hand
				if (lootBag.transform.parent == leftController.transform)
				{
					//Toggle on current hand
					if (lootBag.GetActiveState() >= 0)
					{
						lootBag.SetActiveState(false, 1);
					}
					else
					{
						lootBag.SetActiveState(true, 1);
					}
				}
				else
				{
					//Change hands
					lootBag.SetActiveState(false, 0);
					lootBag.SetActiveState(true, leftController.transform, 1);
				}
			}
		}
		else if (!leftTriggerReleased)
		{
			leftTriggerReleased = true;
		}
	}

	void FindControllerReferencesIfNotYetFound()
	{
		if (rightController == null)
		{
			rightController = VRTK_DeviceFinder.GetControllerRightHand(false);
		}

		if (rightController != null && controllerEventsRight == null)
		{
			controllerEventsRight = rightController.GetComponent<VRTK_ControllerEvents>();
		}

		if (leftController == null)
		{
			leftController = VRTK_DeviceFinder.GetControllerLeftHand(false);
		}

		if (leftController != null && controllerEventsLeft == null)
		{
			controllerEventsLeft = leftController.GetComponent<VRTK_ControllerEvents>();
		}

		if(controllerEventsRight && controllerEventsLeft)
		{
			controllerReferencesFound = true;
		}
	}
}
