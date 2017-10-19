using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PlayerDragMovement : MonoBehaviour
{
    public VRTK_PlayerClimb climbMovement;
    public VRTK_BodyPhysics bodyPhysics;
    public float dragSpeed = 1f;

	private enum State { Active, Inactive, Locked }
    private State dragState = State.Active;
    private Transform playArea;
    private Vector3 controllerDragStartLocalPos= Vector3.zero;
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

        if (!climbMovement)
        {
            climbMovement = GetComponent<VRTK_PlayerClimb>();
        }
	}

    void OnEnable()
    {
        if (climbMovement)
        {
			climbMovement.PlayerClimbStarted += OnPlayerClimbStarted;
            climbMovement.PlayerClimbEnded += OnPlayerClimbEnded;
        }
    }

    void OnDisable()
    {
        if (climbMovement)
        {
			climbMovement.PlayerClimbStarted -= OnPlayerClimbStarted;
            climbMovement.PlayerClimbEnded -= OnPlayerClimbEnded;
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
			bodyPhysics.ResetFalling();
			//bodyPhysics.TogglePreventSnapToFloor(true);
			//bodyPhysics.enableBodyCollisions = false;
			bodyPhysics.ToggleOnGround(false);
			dragState = State.Active;
		}
	}

    void TriggerReleased(object sender, ControllerInteractionEventArgs e)
    {
        GameObject controller = VRTK_DeviceFinder.GetActualController(((VRTK_ControllerEvents)sender).gameObject);
		if (controller == currentUsedController)
        {
			currentUsedController = null;
			//bodyPhysics.TogglePreventSnapToFloor(false);
			//bodyPhysics.enableBodyCollisions = true;
			bodyPhysics.ToggleOnGround(true);
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
			playArea.position = playAreaDragStartPos + ((controllerDragStartLocalPos - currentUsedController.transform.localPosition) * dragSpeed);
        }
	}

	void OnPlayerClimbStarted(object sender, PlayerClimbEventArgs e)
	{
		dragState = State.Locked;
	}

    void OnPlayerClimbEnded(object sender, PlayerClimbEventArgs e)
    {
		dragState = State.Inactive;
	}
}
