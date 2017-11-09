using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;

public class PlayerDragMovement : MonoBehaviour
{
    public VRTK_InteractGrab[] grab;
    public VRTK_BodyPhysics bodyPhysics;
    public float dragSpeed = 1f;

    private enum State { WaitingForInput, WaitingForConfirmation, Active, Locked }
	private State dragState = State.WaitingForInput;
    private Transform playArea;
    private Vector3 controllerDragStartLocalPos = Vector3.zero;
    private Vector3 playAreaDragStartPos = Vector3.zero;
    private GameObject currentUsedController;
	private GameObject currentPressedController;

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
            grab[i].ControllerGrabInteractableObject += OnPlayerGrabObject;
            grab[i].ControllerUngrabInteractableObject += OnPlayerUngrabObject;
            grab[i].GrabButtonPressed += OnPlayerGrabButtonPressed;
            grab[i].GrabButtonReleased += OnPlayerGrabButtonReleased;
        }

        playArea = VRTK_DeviceFinder.PlayAreaTransform();
    }

    void OnDisable()
    {
        for (int i = 0; i < grab.Length; i++)
        {
            grab[i].ControllerGrabInteractableObject -= OnPlayerGrabObject;
            grab[i].ControllerUngrabInteractableObject -= OnPlayerUngrabObject;
            grab[i].GrabButtonPressed -= OnPlayerGrabButtonPressed;
            grab[i].GrabButtonReleased -= OnPlayerGrabButtonReleased;
        }
    }

    void Update()
    {
		if (dragState == State.WaitingForConfirmation)
		{
			BeginDrag();
		}
        if (dragState == State.Active && playArea)
        {
            Vector3 finalPos = playAreaDragStartPos + (playArea.rotation * (controllerDragStartLocalPos - currentUsedController.transform.localPosition) * dragSpeed);
            finalPos.y = playArea.position.y;
            playArea.position = finalPos;
        }
    }

    void BeginDrag()
    {
        dragState = State.Active;
		currentUsedController = currentPressedController;
		controllerDragStartLocalPos = currentUsedController.transform.localPosition;
		playAreaDragStartPos = playArea.position;
	}

    void EndDrag()
    {
        dragState = State.WaitingForInput;
        currentUsedController = null;
    }

    void OnPlayerGrabButtonPressed(object sender, ControllerInteractionEventArgs e)
    {
		currentPressedController = e.controllerReference.actual;
		if (dragState == State.WaitingForInput || dragState == State.Active)
        {			
			dragState = State.WaitingForConfirmation;
        }
    }

    void OnPlayerGrabButtonReleased(object sender, ControllerInteractionEventArgs e)
    {
		if (currentPressedController == e.controllerReference.actual)
		{
			currentPressedController = null;
		}
		if (dragState == State.Active && currentUsedController == e.controllerReference.actual)
        {
            EndDrag();
        }
    }

    void OnPlayerGrabObject(object sender, ObjectInteractEventArgs e)
	{
		VRTK_BaseGrabAttach grabType = e.target.GetComponent<VRTK_BaseGrabAttach>();
		if (grabType && dragState == State.WaitingForConfirmation)
        {
			if (grabType.IsClimbable())
			{
				dragState = State.Locked;
				currentUsedController = e.controllerReference.actual;
			}
			else if (currentUsedController)
			{
				dragState = State.Active;
			}
			else
			{
				dragState = State.WaitingForInput;
			}
		}
    }

    void OnPlayerUngrabObject(object sender, ObjectInteractEventArgs e)
    {
		if (currentUsedController == e.controllerReference.actual)
        {
            dragState = State.WaitingForInput;
			currentUsedController = null;
        }
    }
}
