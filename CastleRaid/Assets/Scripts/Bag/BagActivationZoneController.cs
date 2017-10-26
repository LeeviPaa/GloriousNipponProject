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
    bool initialized = false;
    float interactableGrabCooldown = 0.5f;

    private void Update()
    {
        //Activate any functionality only after the first update loop
        if (!initialized)
        {
            initialized = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (initialized)
        {
            //If the entering collider is a bagActivationTrigger
            if (other.GetComponent<BagActivationTriggerTag>())
            {
                HandAnimationController handAnimationController = other.transform.parent.GetComponentInChildren<HandAnimationController>();
                //If the entering colliders sibling transform contains a HandAnimationController,
                //which is not currently holding an interactable object
                //and has not held an interactable object within the interactableGrabCooldown
                if (handAnimationController
                    && (!handAnimationController.GetIsGrabbingInteractable())
                    && ((Time.time - handAnimationController.GetGrabEndTime()) >= interactableGrabCooldown))
                {
                    Transform controllerRoot = other.GetComponentInParent<VRTK_ControllerEvents>().transform;

                    switch (toggleType)
                    {
                        //If the activation zone toggle type is DEACTIVATE
                        case EToggleType.DEACTIVATE:
                            //If the bag and the entering hand are both the right hand
                            if (primaryLootBag.GetActiveState() == 0 && controllerRoot.GetComponentInChildren<HandAnimationController>().GetHandSide()
                                == HandAnimationController.EHandSide.RIGHT)
                            {
                                //Debug.Log("Bag was active on right hand, disabling");
                                //Disable the primary LootBag
                                primaryLootBag.SetActiveState(false, 0);
                            }
                            //If the bag and the entering hand are both the left hand
                            else if (primaryLootBag.GetActiveState() == 1 && controllerRoot.GetComponentInChildren<HandAnimationController>().GetHandSide()
                                == HandAnimationController.EHandSide.LEFT)
                            {
                                //Debug.Log("Bag was active on left hand, disabling");
                                //Disable the primary LootBag
                                primaryLootBag.SetActiveState(false, 1);
                            }
                            else if (primaryLootBag.GetActiveState() == -1)
                            {
                                //Check which hand is activating the bag
                                switch (controllerRoot.GetComponentInChildren<HandAnimationController>().GetHandSide())
                                {
                                    case HandAnimationController.EHandSide.RIGHT:
                                        //Debug.Log("Bag was inactive, enabling on right hand");
                                        //Enable the primary bag on the right hand
                                        primaryLootBag.SetActiveState(true, controllerRoot, 0);
                                        break;

                                    case HandAnimationController.EHandSide.LEFT:
                                        //Debug.Log("Bag was inactive, enabling on left hand");
                                        //Enable the primary bag on the left hand
                                        primaryLootBag.SetActiveState(true, controllerRoot, 1);
                                        break;

                                    default:
                                        break;
                                }
                            }
                            break;

                        //If the activation zone toggle type is TOGGLE
                        case EToggleType.TOGGLE:
                            //If the bag and the entering hand are both the right hand
                            if (primaryLootBag.GetActiveState() == 0 && controllerRoot.GetComponentInChildren<HandAnimationController>().GetHandSide()
                                == HandAnimationController.EHandSide.RIGHT)
                            {
                                //Debug.Log("Bag was active on right hand, disabling handBag and enabling backBag");
                                //Disable the primary LootBag
                                primaryLootBag.SetActiveState(false, 0);
                                //Enable the secondary LootBag
                                secondaryLootBag.SetActiveState(true);
                            }
                            //If the bag and the entering hand are both the left hand
                            else if (primaryLootBag.GetActiveState() == 1 && controllerRoot.GetComponentInChildren<HandAnimationController>().GetHandSide()
                                == HandAnimationController.EHandSide.LEFT)
                            {
                                //Debug.Log("Bag was active on left hand, disabling handBag and enabling backBag");
                                //Disable the primary LootBag
                                primaryLootBag.SetActiveState(false, 1);
                                //Enable the secondary LootBag
                                secondaryLootBag.SetActiveState(true);
                            }
                            else if (primaryLootBag.GetActiveState() == -1)
                            {
                                //Disable the secondary LootBag
                                secondaryLootBag.SetActiveState(false);

                                //Check which hand is activating the bag
                                switch (controllerRoot.GetComponentInChildren<HandAnimationController>().GetHandSide())
                                {
                                    case HandAnimationController.EHandSide.RIGHT:
                                        //Debug.Log("Bag was inactive, enabling on right hand");
                                        //Enable the primary bag on the right hand
                                        primaryLootBag.SetActiveState(true, controllerRoot, 0);
                                        break;
                                    case HandAnimationController.EHandSide.LEFT:
                                        //Debug.Log("Bag was inactive, enabling on left hand");
                                        //Enable the primary bag on the left hand
                                        primaryLootBag.SetActiveState(true, controllerRoot, 1);
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
                else
                {
                    Debug.Log("HandAnimationController not found, the hand is holding an interactableObject, "
                        + "or it has held an interactable object within the last " + interactableGrabCooldown + " seconds");
                }

            }
        }
    }


}
