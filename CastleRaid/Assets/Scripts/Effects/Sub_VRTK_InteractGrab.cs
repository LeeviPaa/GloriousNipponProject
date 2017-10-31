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

        var e = GetComponent<VRTK_ControllerEvents>();
        if (e)
            e.GripPressed += OnGrabbed;

    }

    protected override void OnDisable()
    {
        base.OnDisable();
        var e = GetComponent<VRTK_ControllerEvents>();
        if (e)
            e.GripPressed -= OnGrabbed;

    }
    private void OnGrabbed(object sender, ControllerInteractionEventArgs e)
    {
        if (grabbedObject)
            if (grabbedObject.GetComponent<VRTK.GrabAttachMechanics.VRTK_ClimbableGrabAttach>())
            {
                print("Viveration!!");
                SteamVR_Controller.Input((int)VRTK_DeviceFinder.GetControllerHand(gameObject)).TriggerHapticPulse(100);
            }
    }
}
