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
    private string hitGroundSound;
    [SerializeField]
    private string teleportSound;

    private Transform playArea;

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

    void OnStopFalling(object sender, BodyPhysicsEventArgs e)
    {
        GameManager.audioManager.GetAudio(hitGroundSound, true, true, playArea.position, transform);
    }

    void OnTeleporting(object sender, DestinationMarkerEventArgs e)
    {
        GameManager.audioManager.GetAudio(hitGroundSound, true, true, playArea.position, playArea.transform);
    }
}
