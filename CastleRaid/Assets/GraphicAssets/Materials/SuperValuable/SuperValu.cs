using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperValu : MonoBehaviour {

    private MeshRenderer thisRend;
    private int counter;
    private Vector2 offset;
    public Vector2 XYoffsetSpeed = Vector2.one;
	void Start () {
        thisRend = transform.GetComponent<MeshRenderer>();
        if(thisRend == null)
        {
            Debug.LogError("Material missing");
        }
	}
	
	// Update is called once per frame
	void Update () {
		if(thisRend != null)
        {
            counter++;
            transform.Rotate(XYoffsetSpeed.x*Time.deltaTime, 0, XYoffsetSpeed.y*Time.deltaTime);
        }
	}
}
