using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ClimbingIndicator : MonoBehaviour
{
    public bool useHaptics = true;
    public bool useAudio = true;

    [SerializeField]
    private VRTK_InteractGrab grab;
    [SerializeField]
    private float hapticDuration = 1f;
    [SerializeField]
    private float hapticStrength = 1f;
    [SerializeField]
    private string grabAudio = "";

    //ControllerInteractionEventArgs? temp = null;

    void Awake()
    {
        if (!grab)
        {
            grab = GetComponent<VRTK_InteractGrab>();
        }

        //if (!GetHand(SDK_BaseController.ControllerHand.Left)||!GetHand( SDK_BaseController.ControllerHand.Right) )
        //{
        //    Debug.LogError("Missing attachment!");
        //}

        //var leftEvents = GetHand(SDK_BaseController.ControllerHand.Left).GetComponent<VRTK_ControllerEvents>();
        //leftEvents.GripPressed += OnGrabbed;

        //var rightEvents = GetHand(SDK_BaseController.ControllerHand.Right).GetComponent<VRTK_ControllerEvents>();
        //rightEvents.GripPressed +=OnGrabbed;
    }
    //GameObject GetHand(SDK_BaseController.ControllerHand hand)
    //{
    //    return hands[(int)hand - 1];
    //}

    protected void OnEnable()
    {
        // controllerRightHandEvents.GripPressed += OnGrabbed;
        grab.ControllerGrabInteractableObject += OnGrabbed;
    }

    protected void OnDisable()
    {
        //  controllerRightHandEvents.GripPressed -= OnGrabbed;
        grab.ControllerGrabInteractableObject -= OnGrabbed;
    }

    private void OnGrabbed(object sender, ObjectInteractEventArgs e)
    {
		//print(e.controllerReference.hand + "Grabbed!");
		//var hand = e.controllerReference.scriptAlias;
		//var handGrab = hand.GetComponent<VRTK_InteractGrab>();
		//if (!handGrab.GetGrabbedObject())
		//{
		//    //print(handGrab.GetGrabbedObject());
		//    return;
		//}
		//if (!handGrab.GetGrabbedObject().GetComponent<VRTK.GrabAttachMechanics.VRTK_ClimbableGrabAttach>())
		//    return;

		//var tracedObject = e.controllerReference.actual.GetComponent<SteamVR_TrackedObject>();

		//if (!tracedObject)
		//{
		//    //print("Not steamVR " + e.controllerReference.actual);
		//    return;
		//}

		// For now the effect require a VRTK_ClimbableGrabAttach to trigger.
		// May cause problems if there are other VRTK_ClimbableGrabAttach object that would not require the effects.
		if (e.target.GetComponent<VRTK.GrabAttachMechanics.VRTK_ClimbableGrabAttach>())
		{
			if (useHaptics)
			{
				StartCoroutine(TriggerHapticPulse(e.controllerReference, hapticDuration));
			}
			if (useAudio)
			{
				GameManager.audioManager.GetAudio(grabAudio, true, true, transform.position, transform);
			}
		}
    }
   
    void Update()
    {
        //if(temp != null)
        //if(temp.Value.controllerReference.actual)
        //{
        //    //print(temp.Value.controllerReference.scriptAlias.GetComponent<VRTK_InteractGrab>().GetGrabbedObject());
        //}
    }

	// This can be copied if haptic feedback is needed elsewhere
	IEnumerator TriggerHapticPulse(VRTK_ControllerReference cont, float duration) 
	{
		while (duration > 0f) 
		{
			VRTK_ControllerHaptics.TriggerHapticPulse(cont, hapticStrength);
			duration -= Time.deltaTime;
			yield return null;
		}
	}
}
