using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PlayerMovementAudio : MonoBehaviour
{
    [SerializeField]
    private VRTK_BodyPhysics bodyPhysics;
    [SerializeField]
    private VRTK_BasicTeleport teleporter;
	[SerializeField]
	private float minimumValidFallDistance;
	[SerializeField]
    private string hitGroundSound;
	[SerializeField]
	private float minimumValidTeleportDistance;
	[SerializeField]
    private string teleportSound;

    private Transform playArea;
	private Vector3 fallStartPos;

    void Awake()
    {
        VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
    }

    void OnEnable()
    {
        playArea = VRTK_DeviceFinder.PlayAreaTransform();

        if (!bodyPhysics)
        {
            bodyPhysics = GetComponent<VRTK_BodyPhysics>();
        }
        if (!teleporter)
        {
            teleporter = GetComponent<VRTK_BasicTeleport>();
        }

        if (bodyPhysics)
        {
			bodyPhysics.StartFalling += OnStartFalling;
			bodyPhysics.StopFalling += OnStopFalling;
		}
        if (teleporter)
        {
            teleporter.Teleporting += OnTeleporting;
        }
    }

    void OnDisable()
    {
        if (bodyPhysics)
        {
			bodyPhysics.StartFalling -= OnStartFalling;
			bodyPhysics.StopFalling -= OnStopFalling;
        }
        if (teleporter)
        {
            teleporter.Teleporting -= OnTeleporting;
        }
    }

	void OnDestroy()
    {
        VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
    }

	void OnStartFalling(object sender, BodyPhysicsEventArgs e)
	{
		fallStartPos = playArea.position;
	}

    void OnStopFalling(object sender, BodyPhysicsEventArgs e)
    {
		float distance = Vector3.Magnitude(fallStartPos - playArea.position);
		if (distance >= minimumValidFallDistance)
		{
			GameManager.audioManager.GetAudio(hitGroundSound, true, true, playArea.position, transform);
		}
	}

    void OnTeleporting(object sender, DestinationMarkerEventArgs e)
    {
		float distance = Vector3.Magnitude(e.destinationPosition - playArea.position);
		print("Teleport distance " + distance);
		if (distance >= minimumValidTeleportDistance)
		{
			GameManager.audioManager.GetAudio(teleportSound, true, true, playArea.position, playArea.transform);
		}	
    }
}
