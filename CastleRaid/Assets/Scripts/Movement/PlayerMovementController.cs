using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PlayerMovementController : MonoBehaviour
{
    public VRTK_PlayerClimb climbMovement;
    public PlayerDragMovement dragMovement;
    public VRTK_DashTeleport dashMovement;

    void Awake()
    {
        if (!climbMovement)
        {
            climbMovement = GetComponent<VRTK_PlayerClimb>();
        }
        if (climbMovement)
        {
			climbMovement.PlayerClimbStarted += OnPlayerClimbStarted;
            climbMovement.PlayerClimbEnded += OnPlayerClimbEnded;
        }
        if (!dragMovement)
        {
            dragMovement = GetComponent<PlayerDragMovement>();
        }
        if (!dashMovement)
        {
            dashMovement = GetComponent<VRTK_DashTeleport>();
        }
    }

    void OnPlayerClimbStarted(object sender, PlayerClimbEventArgs e)
    {
        dragMovement.enabled = false;
    }

    void OnPlayerClimbEnded(object sender, PlayerClimbEventArgs e)
    {
		dragMovement.enabled = true;
    }
}
