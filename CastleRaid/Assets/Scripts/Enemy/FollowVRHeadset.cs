using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class FollowVRHeadset : MonoBehaviour
{
	Transform transformToFollow;
	bool transformToFollowFound = false;
	[SerializeField]
	bool isEnabled = true;

	private void Update()
	{
		if (!transformToFollowFound)
		{
			Transform vrCameraTransform = VRTK_DeviceFinder.HeadsetCamera();

			if (vrCameraTransform != null)
			{
				transformToFollow = vrCameraTransform;
				transformToFollowFound = true;
			}
		}
	}

	private void FixedUpdate()
	{
		if (isEnabled && transformToFollowFound)
		{
			transform.position = transformToFollow.position;
			Vector3 finalRotation = transformToFollow.eulerAngles;
			finalRotation.x = 0;
			finalRotation.z = 0;
			transform.rotation = Quaternion.Euler(finalRotation);
		}
	}
}
