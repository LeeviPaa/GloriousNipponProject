using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour {
    [SerializeField]
    private Rigidbody rb;
	// Use this for initialization
	void Start () {
	}   
	
	// Update is called once per frame
	void Update () {
        transform.position += new Vector3(Input.GetAxis("Horizontal")*0.1f, 0, 0);
        transform.position += new Vector3(0,0,Input.GetAxis("Vertical") * 0.1f);
       
        if(Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * 400);
        }

        print(GetComponent<Rigidbody>().velocity);

    }
}
