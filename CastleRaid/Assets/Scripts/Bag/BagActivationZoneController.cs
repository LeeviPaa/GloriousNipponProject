using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class BagActivationZoneController : MonoBehaviour
{
    enum EToggleType
    {
        DEACTIVATE,
        TOGGLE,
    }

    [SerializeField]
    LootBag primaryLootBag;
    [SerializeField]
    LootBag secondaryLootBag;
    [SerializeField]
    EToggleType toggleType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<VRTK_ControllerEvents>())
        {
            switch (toggleType)
            {
                //If the activation zone toggle type is DEACTIVATE
                case EToggleType.DEACTIVATE:
                    if (primaryLootBag.GetActiveState())
                    {
                        //Disable the primary LootBag
                        primaryLootBag.SetActiveState(false);
                    }
                    else
                    {
                        //Check which hand is activating the bag
                        switch (other.transform.parent.GetComponentInChildren<HandAnimationController>().GetHandSide())
                        {
                            case HandAnimationController.EHandSide.RIGHT:
                                //Enable the primary bag on the right hand
                                primaryLootBag.SetActiveState(true, 0);
                                break;

                            case HandAnimationController.EHandSide.LEFT:
                                //Enable the primary bag on the left hand
                                primaryLootBag.SetActiveState(true, 1);
                                break;

                            default:
                                break;
                        }
                    }
                    break;

                //If the activation zone toggle type is TOGGLE
                case EToggleType.TOGGLE:
                    //If the primary bag is active
                    if (primaryLootBag.GetActiveState())
                    {
                        //Disable the primary LootBag
                        primaryLootBag.SetActiveState(false);
                        //Enable the secondary LootBag
                        secondaryLootBag.SetActiveState(false);

                    }
                    else
                    {
                        //Check which hand is activating the bag
                        switch (other.transform.parent.GetComponentInChildren<HandAnimationController>().GetHandSide())
                        {
                            case HandAnimationController.EHandSide.RIGHT:
                                //Enable the primary bag on the right hand
                                primaryLootBag.SetActiveState(true, 0);
                                break;
                            case HandAnimationController.EHandSide.LEFT:
                                //Enable the primary bag on the left hand
                                primaryLootBag.SetActiveState(true, 1);
                                break;
                            default:
                                break;
                        }
                    }
                    break;

                default:
                    break;
            }
        }
    }


}
