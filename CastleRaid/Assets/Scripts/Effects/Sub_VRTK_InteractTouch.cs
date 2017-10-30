

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Sub_VRTK_InteractTouch : VRTK_InteractTouch
{

    [SerializeField]
    [ColorUsage(false, true, 0f, 10f, 0.125f, 3f)]
    private Color climbableIndicateColor;

    protected override void OnTriggerStay(Collider collider)
    {
        GameObject colliderInteractableObject = TriggerStart(collider);

        if (touchedObject == null || collider.transform.IsChildOf(touchedObject.transform))
        {
            triggerIsColliding = true;
        }

        if (touchedObject == null && colliderInteractableObject != null && IsObjectInteractable(collider.gameObject))
        {
            touchedObject = colliderInteractableObject;
            VRTK_InteractableObject touchedObjectScript = touchedObject.GetComponent<VRTK_InteractableObject>();

            //If this controller is not allowed to touch this interactable object then clean up touch and return before initiating a touch.
            if (touchedObjectScript != null && !touchedObjectScript.IsValidInteractableController(gameObject, touchedObjectScript.allowedTouchControllers))
            {
                CleanupEndTouch();
                return;
            }
            OnControllerStartTouchInteractableObject(SetControllerInteractEvent(touchedObject));
            StoreTouchedObjectColliders(collider);

            if (touchedObject.GetComponent<VRTK.GrabAttachMechanics.VRTK_ClimbableGrabAttach>())
            {
                var color = climbableIndicateColor;
                var mat = touchedObject.GetComponent<Renderer>();
                mat.material.EnableKeyword("_EMISSION");
                mat.material.SetColor("_EmissionColor", color);
            }
            else
            {
                touchedObjectScript.ToggleHighlight(true);
            }
            ToggleControllerVisibility(false);
            CheckRumbleController(touchedObjectScript);
            touchedObjectScript.StartTouching(this);

            OnControllerTouchInteractableObject(SetControllerInteractEvent(touchedObject));
        }

    }

    protected override void StopTouching(GameObject untouched)
    {
        OnControllerStartUntouchInteractableObject(SetControllerInteractEvent(untouched));
        if (IsObjectInteractable(untouched))
        {
            VRTK_InteractableObject untouchedObjectScript = (untouched != null ? untouched.GetComponent<VRTK_InteractableObject>() : null);
            if (untouchedObjectScript != null)
            {
                untouchedObjectScript.StopTouching(this);
                if (!untouchedObjectScript.IsTouched())
                {
                    if (untouched.GetComponent<VRTK.GrabAttachMechanics.VRTK_ClimbableGrabAttach>())
                    {
                        var color = climbableIndicateColor;
                        var mat = untouched.GetComponent<Renderer>();
                        mat.material.DisableKeyword("_EMISSION");
                    }
                    else
                    {
                        untouchedObjectScript.ToggleHighlight(false);
                    }
                }
            }
        }

        ToggleControllerVisibility(true);
        OnControllerUntouchInteractableObject(SetControllerInteractEvent(untouched));
        CleanupEndTouch();
    }
}
