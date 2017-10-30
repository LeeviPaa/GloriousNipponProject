using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Sub_VRTK_InteractTouch : VRTK_InteractTouch {

    protected new void OnTriggerStay(Collider collider)
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

            //touchedObjectScript.ToggleHighlight(true);
            var color = new Color(0, 0, 0);
            var mat = touchedObject.GetComponent<Material>();
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", color);
            ToggleControllerVisibility(false);
            CheckRumbleController(touchedObjectScript);
            touchedObjectScript.StartTouching(this);

            OnControllerTouchInteractableObject(SetControllerInteractEvent(touchedObject));
        }
    }
}
