using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Gadget_HookShot : MonoBehaviour
{
    public VRTK_InteractableObject interaction;

    void Start()
    {
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
        }
        else
        {
            enabled = false;
        }
    }

    void OnGrabBegin(object sender, InteractableObjectEventArgs e)
    {

    }

    void OnGrabEnd(object sender, InteractableObjectEventArgs e)
    {
        
    }

    void OnUseBegin(object sender, InteractableObjectEventArgs e)
    {
        PlayerRelocator relocator = GameManager.levelInstance.GetPlayerScriptHolder().GetComponent<PlayerRelocator>();
        if (relocator)
        {

        }
    }

    void OnUseEnd(object sender, InteractableObjectEventArgs e)
    {

    }
}
