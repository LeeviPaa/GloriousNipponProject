using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class LockBreaker : MonoBehaviour {

	private Rigidbody rb;

	public float GetVelocity() {
		rb = GetComponent<Rigidbody>();
		return rb.velocity.magnitude;
	}
}
