using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Gadget_Gyrocopter : MonoBehaviour {
	[SerializeField]
	Transform propellerTransform;
	VRTK_InteractableObject interactableScript;
	Rigidbody copterRigidbody;
	Rigidbody grabberRigidbody;
	bool isUsed = false;
	[SerializeField]
	float liftForce = 1200f;
	[SerializeField]
	float cameraRigLiftForceMultiplier = 100f;
	[SerializeField]
	float flyVelocityMagnitudeLimit = 5f;
	float rotateSpeed = 1080f;

	private void Awake() {
		interactableScript = GetComponent<VRTK_InteractableObject>();
		interactableScript.InteractableObjectUsed += OnUse;
		interactableScript.InteractableObjectUnused += OnUnuse;

		copterRigidbody = GetComponent<Rigidbody>();
	}

	private void Update() {
		if (isUsed) {
			Vector3 newPropellerRotation = Vector3.zero;
			newPropellerRotation.y = rotateSpeed * Time.deltaTime;
			propellerTransform.rotation *= Quaternion.Euler(newPropellerRotation);
		}
	}

	private void FixedUpdate() {
		if (isUsed) {
			if (grabberRigidbody != null)
			{
				Debug.Log("grabberRigidbody is not null, adding force to it");
				grabberRigidbody.isKinematic = false;
				grabberRigidbody.AddForce(transform.up * liftForce * cameraRigLiftForceMultiplier * Time.fixedDeltaTime);

				//LimitVelocity(grabberRigidbody, flyVelocityMagnitudeLimit);
			} else
			{
				Debug.Log("grabberRigidbody was null, adding force to copterRigidbody");
				copterRigidbody.AddForce(transform.up * liftForce * Time.fixedDeltaTime);

				//LimitVelocity(copterRigidbody, flyVelocityMagnitudeLimit);
			}
		}
	}

	private void LimitVelocity(Rigidbody rb, float velMagLimit)
	{
		Debug.Log("rb.velocity.magnitude: " + rb.velocity.magnitude);
		if(rb.velocity.magnitude > velMagLimit)
		{
			rb.velocity = rb.velocity.normalized * velMagLimit;
			Debug.Log("rb.velocity.magnitude after limiting: " + rb.velocity.magnitude);
		}
	}

	private void OnUse(object sender, InteractableObjectEventArgs e) {

		VRTK_PlayerObject playerObjectScript = e.interactingObject.GetComponentInParent<VRTK_PlayerObject>();
		if (playerObjectScript != null) {

			if (playerObjectScript.objectType == VRTK_PlayerObject.ObjectTypes.CameraRig) {
				grabberRigidbody = playerObjectScript.GetComponent<Rigidbody>();

			} else if (grabberRigidbody == null) {
				playerObjectScript = playerObjectScript.transform.parent.GetComponentInParent<VRTK_PlayerObject>();

				if (playerObjectScript != null) {

					if (playerObjectScript.objectType == VRTK_PlayerObject.ObjectTypes.CameraRig) {
						grabberRigidbody = playerObjectScript.GetComponent<Rigidbody>();
					}
				}
			}
		}

		if (grabberRigidbody != null)
		{
			grabberRigidbody.isKinematic = false;
			grabberRigidbody.AddForce((transform.up * liftForce * cameraRigLiftForceMultiplier * Time.fixedDeltaTime) / 8f, ForceMode.Impulse);
		}
		else
		{
			copterRigidbody.AddForce((transform.up * liftForce * Time.fixedDeltaTime) / 8f, ForceMode.Impulse);		
		}

		isUsed = true;
	}

	private void OnUnuse(object sender, InteractableObjectEventArgs e) {
		isUsed = false;
		grabberRigidbody = null;
	}

}
