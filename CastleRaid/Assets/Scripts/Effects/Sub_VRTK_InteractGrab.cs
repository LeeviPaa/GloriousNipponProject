using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
public class Sub_VRTK_InteractGrab : VRTK_InteractGrab
{
    void Start()
    {
        var device0 = SteamVR_Controller.Input(0);
        device0.TriggerHapticPulse(2000);
        var device1 = SteamVR_Controller.Input(1);
        device1.TriggerHapticPulse(2000);
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
                SDK_BaseController.ControllerHand hand = VRTK_DeviceFinder.GetControllerHand(gameObject);
                var device = SteamVR_Controller.Input((int)hand);
                device.TriggerHapticPulse(2000);
                print(device.ToString()+hand.ToString() + " Viveration!!");
            }
    }
}
