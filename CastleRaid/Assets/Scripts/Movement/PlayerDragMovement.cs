using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PlayerDragMovement : MonoBehaviour
{
    public VRTK_InteractGrab[] grab;
    public VRTK_BodyPhysics bodyPhysics;
    public float dragSpeed = 1f;

    private enum State { Active, Inactive, Locked }
    private State dragState = State.Active;
    private Transform playArea;
    private Vector3 controllerDragStartLocalPos = Vector3.zero;
    private Vector3 playAreaDragStartPos = Vector3.zero;
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
        if (!bodyPhysics)
        {
            enabled = false;
        }
    }

    void OnEnable()
    {
        for (int i = 0; i < grab.Length; i++)
        {
            grab[i].ControllerGrabInteractableObject += OnPlayerGrab;
            grab[i].ControllerUngrabInteractableObject += OnPlayerUngrab;
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < grab.Length; i++)
        {
            grab[i].ControllerGrabInteractableObject -= OnPlayerGrab;
            grab[i].ControllerUngrabInteractableObject -= OnPlayerUngrab;
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
                controllerEvents.GripPressed += GripPressed;
                controllerEvents.GripReleased += GripReleased;
            }
            else
            {
                controllerEvents.GripPressed -= GripPressed;
                controllerEvents.GripReleased -= GripReleased;
            }
        }
    }

    void GripPressed(object sender, ControllerInteractionEventArgs e)
    {
		if (dragState == State.Locked)
		{
			return;
		}
		GameObject controller = VRTK_DeviceFinder.GetActualController(((VRTK_ControllerEvents)sender).gameObject);
		if (bodyPhysics.OnGround()) 
		{
			currentUsedController = controller;
			controllerDragStartLocalPos = currentUsedController.transform.localPosition;
			playAreaDragStartPos = playArea.position;
			//bodyPhysics.ResetFalling();
			//bodyPhysics.TogglePreventSnapToFloor(true);
			//bodyPhysics.enableBodyCollisions = false;
			//bodyPhysics.ToggleOnGround(false);
			dragState = State.Active;
		}
	}

    void GripReleased(object sender, ControllerInteractionEventArgs e)
    {
        GameObject controller = VRTK_DeviceFinder.GetActualController(((VRTK_ControllerEvents)sender).gameObject);
		if (controller == currentUsedController)
        {
			currentUsedController = null;
			//bodyPhysics.TogglePreventSnapToFloor(false);
			//bodyPhysics.enableBodyCollisions = true;
			//bodyPhysics.ToggleOnGround(true);
			if (dragState == State.Active)
			{
				dragState = State.Inactive;
			}
		}
    }

    void Update()
    {
        if (dragState == State.Active && playArea)
        {
			Vector3 finalPos = playAreaDragStartPos + ((controllerDragStartLocalPos - currentUsedController.transform.localPosition) * dragSpeed);
			finalPos.y = playArea.position.y;
			playArea.position = finalPos;
            bodyPhysics.ForceSnapToFloor();
        }
	}

	void OnPlayerGrab(object sender, ObjectInteractEventArgs e)
	{
		dragState = State.Locked;
	}

    void OnPlayerUngrab(object sender, ObjectInteractEventArgs e)
    {
		dragState = State.Inactive;
	}
}
