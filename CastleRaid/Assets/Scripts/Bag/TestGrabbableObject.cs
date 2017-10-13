using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGrabbableObject : VRTK.VRTK_InteractableObject {
    public enum EItemID
    {
        small,
        normal,
        big
    }
    public EItemID id;
	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    override protected void Update () {
        base.Update();
        if (IsGrabbed())
            GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
        else
            GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1);

    }
}
