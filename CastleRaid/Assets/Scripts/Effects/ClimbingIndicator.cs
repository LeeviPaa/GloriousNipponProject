using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ClimbingIndicator : MonoBehaviour
{
    [SerializeField]
    VRTK_InteractGrab[] grabs;

    bool enableViveration = true;
    [SerializeField]
    AudioClip[] grabSound;
    AudioSource audioSource;

    ControllerInteractionEventArgs? temp = null;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
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
    public void SetActiveControllerViveration(bool value)
    {
        enableViveration = value;
    }
    protected void OnEnable()
    {
        // controllerRightHandEvents.GripPressed += OnGrabbed;
        foreach (var g in grabs)
            g.ControllerGrabInteractableObject += OnGrabbed;
    }

    protected void OnDisable()
    {
        //  controllerRightHandEvents.GripPressed -= OnGrabbed;

    }

    private void OnGrabbed(object sender, ObjectInteractEventArgs e)
    {
        print(e.controllerReference.hand + "Grabbed!");
        var hand = e.controllerReference.scriptAlias;
        var handGrab = hand.GetComponent<VRTK_InteractGrab>();
        if (!handGrab.GetGrabbedObject())
        {
            print(handGrab.GetGrabbedObject());
            return;
        }
        if (!handGrab.GetGrabbedObject().GetComponent<VRTK.GrabAttachMechanics.VRTK_ClimbableGrabAttach>())
            return;

        var tracedObject = e.controllerReference.actual.GetComponent<SteamVR_TrackedObject>();

        if (!tracedObject)
        {
            print("Not steamVR " + e.controllerReference.actual);
            return;
        }

        if (enableViveration)
        {
            var device = SteamVR_Controller.Input((int)tracedObject.index);
            device.TriggerHapticPulse(2000);
            print(device.ToString() + " Viveration!!");
        }

        audioSource.PlayOneShot(grabSound[Random.Range(0, grabSound.Length)]);
    }
   
    void Update()
    {
        if(temp != null)
        if(temp.Value.controllerReference.actual)
        {
            print(temp.Value.controllerReference.scriptAlias.GetComponent<VRTK_InteractGrab>().GetGrabbedObject());
        }
    }
}
