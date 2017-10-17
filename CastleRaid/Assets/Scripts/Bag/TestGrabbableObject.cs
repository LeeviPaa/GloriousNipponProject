using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class TestGrabbableObject : VRTK_InteractableObject
{
    public enum EItemID
    {
        small,
        normal,
        big
    }
    public EItemID id;
    public AudioClip clip;
    bool isGrabbed = false;

    private BoxCollider bc;
    // Use this for initialization
    void Start()
    {
        bc = GetComponent<BoxCollider>();


    }
    override protected void OnEnable()
    {
        base.OnEnable();
        VRTK_InteractGrab asd = GetComponent<VRTK_InteractGrab>();
        asd.ControllerStartGrabInteractableObject += OnStartGrab;
        asd.ControllerUngrabInteractableObject += OnEndGrab;
    }
    override protected void OnDisable()
    {
        base.OnDisable();
        VRTK_InteractGrab asd = GetComponent<VRTK_InteractGrab>();
        asd.ControllerStartGrabInteractableObject -= OnStartGrab;
        asd.ControllerUngrabInteractableObject -= OnEndGrab;
    }
    void OnStartGrab(object sender, ObjectInteractEventArgs e)
    {
        print("Grab");
        isGrabbed = true;

        // For test;
        GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
        bc.isTrigger = true;
    }

    void OnEndGrab(object sender, ObjectInteractEventArgs e)
    {
        print("Released");
        isGrabbed = false;

        // For test
        GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1);
        bc.isTrigger = false;
    }

    public bool GetIsGrabbed()
    {
        return isGrabbed;
    }


}
