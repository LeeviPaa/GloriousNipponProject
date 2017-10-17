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

    private BoxCollider bc;
	// Use this for initialization
	void Start () {
        bc = GetComponent<BoxCollider>();
	}

    // Update is called once per frame
    override protected void Update () {
        base.Update();
        if (IsGrabbed())
        {
            GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
            bc.isTrigger = true;
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1);
            bc.isTrigger = false;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.GetComponent<Bag>())
        {
        }
    }

    void OnTriggerStay(Collider other)
    {

    }

    void OnTriggerExit(Collider other)
    {
            //if(other.gameObject.GetComponent<Bag>())
            //{
            //    GetComponent<BoxCollider>().isTrigger = false;
            //}
    }
}
