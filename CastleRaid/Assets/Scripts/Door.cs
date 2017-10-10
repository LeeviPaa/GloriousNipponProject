using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

	// KeyLockType
	public enum kLockType {
		NONE,
		BREAKABLE, // User can break
		NONBREAKABLE, // User cant break
	}

	// Variables
	[SerializeField]
	GameObject key_ = null; // Reference key
	[SerializeField]
	kLockType lockType_ = kLockType.NONE; // Type of lock
	[SerializeField]
	bool initialState_ = false; // if this state is true, user can open door.

	bool isLerping_;
	float timeStartedLerping;
	private float animationTime = 1.0f;
	Vector3 startPosOfLerp;
	Vector3 EndPosOfLerp;

	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	private void StartLeap() {
		isLerping_ = true;
		timeStartedLerping = Time.time;

		startPosOfLerp = transform.localPosition;
		Vector3 endPos = transform.localPosition;
		endPos.y += 3.0f;
		EndPosOfLerp = endPos;
	}

	private void LerpDoor() {
		float timeSinceStarted = Time.time - timeStartedLerping;
		float percentageComplete = timeSinceStarted / animationTime;

		transform.position = Vector3.Lerp(startPosOfLerp, EndPosOfLerp, percentageComplete);
		if (percentageComplete >= 1.0f) {
			isLerping_ = false;
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (other) {

		}
	}

	// Open door
	// This code is test
	private void Open() {
		//transform.position.y;
	}
}
