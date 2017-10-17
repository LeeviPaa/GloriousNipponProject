using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

[RequireComponent(typeof(LootBag))]
public class LootBagInputManager : MonoBehaviour
{
    LootBag lootBag;

    VRTK_ControllerEvents controllerEventsRight;
    VRTK_ControllerEvents controllerEventsLeft;

    private void OnEnable()
    {
        //controllerEventsRight = VRTK_DeviceFinder.GetControllerRightHand(false).GetComponent<VRTK_ControllerEvents>();
        //controllerEventsLeft = VRTK_DeviceFinder.GetControllerLeftHand(false).GetComponent<VRTK_ControllerEvents>();

        //Debug.Log("controllerEventsRight: " + controllerEventsRight);
        Debug.Log("Test this stuff on the VR machine");

        lootBag = GetComponent<LootBag>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            lootBag.SetActiveState(!lootBag.GetActiveState());
        }
    }
}
