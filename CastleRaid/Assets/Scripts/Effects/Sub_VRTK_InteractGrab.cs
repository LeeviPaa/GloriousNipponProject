using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
public class Sub_VRTK_InteractGrab : VRTK_InteractGrab
{
    void Start()
    {

    }
    protected override void OnEnable()
    {
        base.OnEnable();

        GetComponent<VRTK_ControllerEvents>().GripPressed += OnGrabbed;
        
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GetComponent<VRTK_ControllerEvents>().GripPressed -= OnGrabbed;

    }
    private void OnGrabbed(object sender, ControllerInteractionEventArgs e)
    {
        print("Grabbed");
    }
}
