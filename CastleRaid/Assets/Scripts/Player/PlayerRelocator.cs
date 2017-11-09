using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PlayerRelocator : MonoBehaviour
{
    public VRTK_BodyPhysics bodyPhysics;
    public VRTK_BasicTeleport teleporter;
    public VRTK_PlayerClimb climb;
    public float relocationFadeDuration;

    private bool movementEnabled = true;
    private Transform playArea;

    void Awake()
    {
        VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);

        if (!bodyPhysics)
        {
            bodyPhysics = GetComponent<VRTK_BodyPhysics>();
        }
        if (!teleporter)
        {
            teleporter = GetComponent<VRTK_BasicTeleport>();
        }
        if (!climb)
        {
            climb = GetComponent<VRTK_PlayerClimb>();
        }
    }

    void OnEnable()
    {
        playArea = VRTK_DeviceFinder.PlayAreaTransform();
    }

    void OnDisable()
    {

    }

    void OnDestroy()
    {
        VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
    }

    public bool isMovementEnabled()
    {
        return movementEnabled;
    }

    public void DisableMovement()
    {
        if (bodyPhysics)
        {
            bodyPhysics.enabled = false;
        }
        if (teleporter)
        {
            bodyPhysics.enabled = false;
        }
        if (climb)
        {
            bodyPhysics.enabled = false;
        }
    }

    public void EnableMovement()
    {
        if (bodyPhysics)
        {
            bodyPhysics.enabled = true;
        }
        if (teleporter)
        {
            bodyPhysics.enabled = true;
        }
        if (climb)
        {
            bodyPhysics.enabled = true;
        }
    }

    public void RelocatePlayer(Vector3 pos, Quaternion rot)
    {
        GameManager.levelInstance.ScreenFade(relocationFadeDuration / 2);
        playArea.position = pos;
        playArea.rotation = rot;
        GameManager.levelInstance.ScreenUnfade(relocationFadeDuration / 2);
    }

    public void RotatePlayArea(float degreesPerSec)
    {
        playArea.transform.rotation *= Quaternion.Euler(0f, degreesPerSec * Time.deltaTime, 0f);
    }

    public Transform GetPlayArea()
    {
        return playArea;
    }
}
