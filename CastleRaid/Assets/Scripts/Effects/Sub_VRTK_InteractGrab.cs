using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
public class Sub_VRTK_InteractGrab : VRTK_InteractGrab
{
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
        if (!grabbedObject && 
            !grabbedObject.GetComponent<VRTK.GrabAttachMechanics.VRTK_ClimbableGrabAttach>())
            return;

        var a = transform.parent.GetComponent<SteamVR_TrackedObject>();

        if (!a) return;

        var device = SteamVR_Controller.Input((int)a.index);
        device.TriggerHapticPulse(2000);
        print(device.ToString() + " Viveration!!");
    }
}
