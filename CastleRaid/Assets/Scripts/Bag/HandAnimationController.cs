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
    VRTK_InteractGrab controllerGrabScript;
    bool controllerReferenceFound = false;

	bool holdingLootBag;

	float closedValueDelta = 10f;
	float closedValueThreshold = 1f;
	float openValueThreshold = 0f;
	int closingState = 0; //0 = not currently changing, 1 = closing, 2 = opening
	float closedValue = -1f;

    bool isGrabbingInteractable = false;
    float grabEndTime = 0f;

	private void OnEnable()
	{
		closingState = 0;
		closedValue = 0f;
        isGrabbingInteractable = false;
        grabEndTime = Time.time;
        handAnimator = GetComponent<Animator>();
		//lootBag.OnLootBagActiveStateChange += OnLootBagActiveStateChange;

		controllerReferenceFound = false;
	}

	private void OnDisable()
	{
		//lootBag.OnLootBagActiveStateChange -= OnLootBagActiveStateChange;

		if (controllerEvents)
		{
			controllerEvents.GripPressed -= OnGripPressed;
			controllerEvents.GripReleased -= OnGripReleased;
        }

        if (controllerGrabScript)
        {
            controllerGrabScript.ControllerGrabInteractableObject -= OnControllerGrabInteractableObject;
            controllerGrabScript.ControllerUngrabInteractableObject -= OnControllerUngrabInteractableObject;
        }
    }

    private void Update()
	{
		if (!controllerReferenceFound)
		{
			FindControllerReferencesIfNotYetFound();

			if (controllerReferenceFound)
			{
                Debug.Log("This should only happen once!");
				controllerEvents.GripPressed -= OnGripPressed;
				controllerEvents.GripPressed += OnGripPressed;
				controllerEvents.GripReleased -= OnGripReleased;
				controllerEvents.GripReleased += OnGripReleased;

                controllerGrabScript.ControllerGrabInteractableObject -= OnControllerGrabInteractableObject;
                controllerGrabScript.ControllerGrabInteractableObject += OnControllerGrabInteractableObject;
                controllerGrabScript.ControllerUngrabInteractableObject -= OnControllerUngrabInteractableObject;
                controllerGrabScript.ControllerUngrabInteractableObject += OnControllerUngrabInteractableObject;
            }
		}

		if (closingState == 1)
		{
			closedValue += closedValueDelta * Time.deltaTime;

			if (closedValue >= closedValueThreshold)
			{
				closingState = 0;
				closedValue = closedValueThreshold;
			}

			handAnimator.SetFloat("ClosedValue", closedValue);
		}
		else if (closingState == 2)
		{
			closedValue -= closedValueDelta * Time.deltaTime;

			if (closedValue <= openValueThreshold)
			{
				closingState = 0;
				closedValue = openValueThreshold;
			}

			handAnimator.SetFloat("ClosedValue", closedValue);
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
            controllerGrabScript = controllerObject.GetComponent<VRTK_InteractGrab>();

        }

		if (controllerEvents && controllerGrabScript)
		{
			controllerReferenceFound = true;
		}
	}

	private void OnGripPressed(object sender, ControllerInteractionEventArgs e)
	{
		//if (!holdingLootBag)
		//{
		//	//Close hand
		//	handAnimator.Play("Hand_Closed");
		//}

		closingState = 1;
	}

	private void OnGripReleased(object sender, ControllerInteractionEventArgs e)
	{
		//if (!holdingLootBag)
		//{
		//	//Open hand
		//	handAnimator.Play("Hand_Idle");
		//}

		closingState = 2;
    }

	private void OnLootBagActiveStateChange(bool bagActive, int bagMesh)
	{
		if (bagMesh == (int)handSide)
		{
			if (bagActive)
			{
				holdingLootBag = true;

				//Close hand
				//handAnimator.Play("Hand_Closed");

				//if (controllerObject)
				//{
				//	VRTK_InteractGrab grab = controllerObject.GetComponent<VRTK_InteractGrab>();
				//	if (grab)
				//	{
				//		grab.enabled = false;
				//	}
				//	VRTK_InteractTouch touch = controllerObject.GetComponent<VRTK_InteractTouch>();
				//	if (touch)
				//	{
				//		touch.enabled = false;
				//	}
				//}
			}
			else
			{
				holdingLootBag = false;

				//Close hand
				//handAnimator.Play("Hand_Idle");

				//if (controllerObject)
				//{
				//	VRTK_InteractGrab grab = controllerObject.GetComponent<VRTK_InteractGrab>();
				//	if (grab)
				//	{
				//		grab.enabled = true;
				//	}
				//	VRTK_InteractTouch touch = controllerObject.GetComponent<VRTK_InteractTouch>();
				//	if (touch)
				//	{
				//		touch.enabled = true;
				//	}
				//}
			}
		}
    }

    void OnControllerGrabInteractableObject(object sender, ObjectInteractEventArgs e)
    {
        isGrabbingInteractable = true;
    }

    void OnControllerUngrabInteractableObject(object sender, ObjectInteractEventArgs e)
    {
        grabEndTime = Time.time;
        isGrabbingInteractable = false;
    }

    public EHandSide GetHandSide()
    {
        return handSide;
    }

    public bool GetIsGrabbingInteractable()
    {
        return isGrabbingInteractable;
    }

    public float GetGrabEndTime()
    {
        return grabEndTime;
    }

}
