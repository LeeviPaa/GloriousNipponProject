using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGrabbableObject : MonoBehaviour {
    public bool isGrabing;
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
	void Update () {
        if (isGrabing)
            GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
        else
            GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1);

    }
}
