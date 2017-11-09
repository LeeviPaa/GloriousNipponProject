using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
	[SerializeField]
	GameObject doorObject;
	HingeJoint hinge;
	[SerializeField]
	Key[] compatibleKeys;
	[SerializeField]
	ELockType lockType;
	[SerializeField]
	bool initialLockState = false;
	[SerializeField]
	bool useLimits = true;
	JointLimits defaultHingeLimits;
	JointLimits lockedHingeLimits;
	float lockedHingeLimit = 0f;
	[SerializeField]
	float lockBreakVelocityThreshold;

	bool locked = false;
	bool isBroken = false;

	bool lerpingToLockedRotation = false;
	float lockingLerpDuration = 0.25f;
	float lockingLerpStartTime = -1f;
	Quaternion lockingLerpStartRotation = Quaternion.identity;
	Quaternion lockingLerpEndRotation = Quaternion.identity;

	private void Awake() {
		//If the door has a lock, initialize relevant variables / states
		if (lockType != ELockType.NONE) {
			//Print a warning if a key is set but the lock type is NONE
			if (compatibleKeys.Length > 0) {
				if (compatibleKeys[0] != null) {
					Debug.LogWarning("Door has a key but no lock!");
				}
			}

			//If the doorObject is null, set current gameObject as the doorObject
			if (doorObject == null) {
				doorObject = gameObject;
			}

			hinge = doorObject.GetComponent<HingeJoint>();
			defaultHingeLimits = hinge.limits;
			lockedHingeLimits.min = -lockedHingeLimit;
			lockedHingeLimits.max = lockedHingeLimit;
			lockingLerpEndRotation = doorObject.transform.localRotation;

			locked = initialLockState;

			if (locked) {
				Lock(false);
			} else {
				Unlock();
			}
		} else {
			//Print a warning if the door has a lock but no key
			if (compatibleKeys.Length == 0) {
				Debug.LogWarning("Door has a lock but no key!");
			} else if (compatibleKeys[0] == null) {
				Debug.LogWarning("Door has a lock but no key!");
			}

			Unlock();
		}
	}

	private void Lock(bool lerpToPosition) {
		if (!isBroken) {
			if (lerpToPosition) {
				lockingLerpStartTime = Time.time;
				lockingLerpStartRotation = doorObject.transform.localRotation;
				lerpingToLockedRotation = true;
			} else {
				hinge.limits = lockedHingeLimits;
				hinge.useLimits = true;

				locked = true;
				//Debug.Log("Door locked!");
			}
		} else {
			Debug.Log("Door Can not Lock, Lock is Broken");
		}
	}

	private void Unlock() {
		lerpingToLockedRotation = false;
		hinge.limits = defaultHingeLimits;
		hinge.useLimits = useLimits;

		locked = false;
		Debug.Log("Door unlocked!");
	}

	void LerpToLockedRotation() {
		float timeSinceStarted = Time.time - lockingLerpStartTime;
		float percentageCompleted = timeSinceStarted / lockingLerpDuration;
		Quaternion doorRotation = Quaternion.Slerp(lockingLerpStartRotation, lockingLerpEndRotation, percentageCompleted);
		doorObject.transform.localRotation = doorRotation;

		if (percentageCompleted >= 1) {
			lerpingToLockedRotation = false;
			Lock(false);
		}
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.K)) {
			if (!lerpingToLockedRotation) {
				if (locked) {
					Unlock();
				} else {
					Lock(true);
				}
			}
		}

		if (lerpingToLockedRotation) {
			LerpToLockedRotation();
		}
	}

	private void OnTriggerEnter(Collider other) {
		//If the door has a lock and it is currently locked
		if (lockType != ELockType.NONE && locked) {
			//If the object entering trigger has the key script as a component
			if (other.GetComponent<Key>()) {
				//Get a reference to the key script and compare it with the compatible keys list
				Key usedKey = other.GetComponent<Key>();

				bool isCompatible = false;
				float count = compatibleKeys.Length;
				for (int i = 0; i < count; i++) {
					if (compatibleKeys[i] == usedKey) {
						isCompatible = true;
						break;
					}
				}

				//If the key was compatible, open door
				if (isCompatible) {
					Debug.Log("Compatible key used, unlocking door!");
					Unlock();
				} else {
					Debug.Log("Tried to use uncompatible key!");
				}
			}
		}
		if (lockType == ELockType.BREAKABLE) {
			if (other.GetComponent<LockBreaker>()) {
				LockBreaker lockBreaker = other.GetComponent<LockBreaker>();
				float velocity = lockBreaker.GetVelocity();
				if (velocity >= lockBreakVelocityThreshold) {
					Break();
				}
			}
		}
	}

	private void Break() {
		isBroken = true;
		Debug.Log("Break Lock");
		Unlock();
	}


}

public enum ELockType {
	NONE,
	BREAKABLE,
	NONBREAKABLE,
}
