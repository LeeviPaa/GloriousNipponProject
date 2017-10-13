using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PlayerDragMovement : MonoBehaviour
{
    public VRTK_BodyPhysics bodyPhysics;
    public float dragSpeed = 1f;

    private bool isDragging = false;
    private Transform playArea;
    private Vector3 controllerDragStartPosition = Vector3.zero;
    private Vector3 playerDragStartPosition = Vector3.zero;
    private GameObject currentUsedController;

    public struct PlayerDragMovementEventArgs
    {

    }

    public delegate void PlayerDragMovementEventHandler(object sender, PlayerDragMovementEventArgs e);
    public event PlayerDragMovementEventHandler playerDragMovementStarted;
    public event PlayerDragMovementEventHandler playerDragMovementEnded;

    void Awake()
    {
        if (!bodyPhysics)
        {
            bodyPhysics = GetComponent<VRTK_BodyPhysics>();
        }
        if (bodyPhysics)
        {

        }
        else
        {
            enabled = false;
        }
    }

	void Start() 
	{
		StartCoroutine(DelayedStart());
	}

	IEnumerator DelayedStart() 
	{
		yield return null;

		playArea = VRTK_DeviceFinder.PlayAreaTransform();

		InitControllerListeners(VRTK_DeviceFinder.GetControllerLeftHand(), true);
		InitControllerListeners(VRTK_DeviceFinder.GetControllerRightHand(), true);
	}

    void InitControllerListeners(GameObject controller, bool state)
    {
		if (controller)
        {
            VRTK_ControllerEvents controllerEvents = controller.GetComponent<VRTK_ControllerEvents>();
            if (state)
            {
                controllerEvents.TriggerPressed += TriggerPressed;
                controllerEvents.TriggerReleased += TriggerReleased;
            }
            else
            {
                controllerEvents.TriggerPressed -= TriggerPressed;
                controllerEvents.TriggerReleased -= TriggerReleased;
            }
        }
    }

    void TriggerPressed(object sender, ControllerInteractionEventArgs e)
    {
		GameObject controller = ((VRTK_ControllerEvents)sender).gameObject;
		if (bodyPhysics.OnGround()) 
		{
			currentUsedController = controller;
			isDragging = true;
			controllerDragStartPosition = currentUsedController.transform.position;
			playerDragStartPosition = playArea.position;
			bodyPhysics.ResetFalling();
			bodyPhysics.TogglePreventSnapToFloor(true);
			bodyPhysics.enableBodyCollisions = false;
			bodyPhysics.ToggleOnGround(false);
		}
    }

    void TriggerReleased(object sender, ControllerInteractionEventArgs e)
    {
        GameObject controller = ((VRTK_ControllerEvents)sender).gameObject;
        if (controller == currentUsedController)
        {
			isDragging = false;
            currentUsedController = null;
			bodyPhysics.TogglePreventSnapToFloor(false);
			bodyPhysics.enableBodyCollisions = true;
			bodyPhysics.ToggleOnGround(true);
		}
    }

    void Update()
    {
        if (isDragging && playArea)
        {
            playArea.position = playerDragStartPosition + ((controllerDragStartPosition - currentUsedController.transform.position) * dragSpeed);
        }
	}
}
