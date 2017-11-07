using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;

public class Gadget_HookShot : MonoBehaviour
{
    public VRTK_InteractableObject interaction;
    public VRTK_ClimbableGrabAttach climbAttach;
    public VRTK_FixedJointGrabAttach fixedAttach;

    public float moveTime;
    public Vector3 startPos;
    public Vector3 endPos;

    void Start()
    {
        if (!climbAttach)
        {
            climbAttach = GetComponent<VRTK_ClimbableGrabAttach>();
            if (!climbAttach)
            {
                climbAttach = gameObject.AddComponent<VRTK_ClimbableGrabAttach>();
            }
        }
        if (!fixedAttach)
        {
            fixedAttach = GetComponent<VRTK_FixedJointGrabAttach>();
            if (!fixedAttach)
            {
                fixedAttach = gameObject.AddComponent<VRTK_FixedJointGrabAttach>();
            }
        }

        if (!interaction)
        {
            interaction = GetComponent<VRTK_InteractableObject>();
        }

        if (interaction)
        {
            interaction.InteractableObjectUsed += OnUseBegin;
            interaction.InteractableObjectUnused += OnUseEnd;
            interaction.InteractableObjectGrabbed += OnGrabBegin;
            interaction.InteractableObjectUngrabbed += OnGrabEnd;

            interaction.grabAttachMechanicScript = fixedAttach;
        }
        else
        {
            enabled = false;
        }

    }

    void Update()
    {

    }

    void OnGrabBegin(object sender, InteractableObjectEventArgs e)
    {

    }

    void OnGrabEnd(object sender, InteractableObjectEventArgs e)
    {
        
    }

    void OnUseBegin(object sender, InteractableObjectEventArgs e)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1000f, LayerMask.NameToLayer("Default")))
        {
            SetPlayerMovement(false);
            interaction.grabAttachMechanicScript = climbAttach;
            //climbAttach.sta
            //startPos = transform.position.
        }
    }

    void OnUseEnd(object sender, InteractableObjectEventArgs e)
    {
        interaction.grabAttachMechanicScript = fixedAttach;
        SetPlayerMovement(true);
    }

    void SetPlayerMovement(bool active)
    {
        PlayerRelocator relocator = GameManager.levelInstance.GetPlayerScriptHolder().GetComponent<PlayerRelocator>();
        if (relocator)
        {
            if (active)
            {
                relocator.EnableMovement();
            }
            else
            {
                relocator.DisableMovement();
            }
        }
    }
}
