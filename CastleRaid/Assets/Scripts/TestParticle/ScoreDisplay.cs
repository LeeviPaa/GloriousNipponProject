using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{

	[SerializeField]
	float fadeTime = 2.0f;
	[SerializeField]
	float upScale = 5.0f;
	bool isFadeing = false;


	// Use this for initialization
	void Start()
	{
		DisplayScore(300);
	}

	// Update is called once per frame
	void Update()
	{
		// same as billborad
		Transform vrCamera = VRTK.VRTK_DeviceFinder.HeadsetCamera();
		Transform cameraTrans = vrCamera.transform;
		transform.forward = cameraTrans.forward;

		if (isFadeing)
		{
			Fadeout();
			MoveUp();
		}
	}

	public void DisplayScore(int value)
	{
		ChangeText(value.ToString());
		isFadeing = true;
	}

	void Fadeout()
	{
		var elpasedTime = Time.deltaTime;
		elpasedTime /= fadeTime;
		var textMesh = GetComponent<TextMesh>();
		var changedColor = textMesh.color;
		changedColor.a -= elpasedTime;
		textMesh.color = changedColor;
		Debug.Log(textMesh.color.a);
	}

	void MoveUp()
	{
		var localPos = transform.localPosition;
		localPos.y += (upScale * Time.deltaTime);
		transform.localPosition = localPos;
	}

	void ChangeText(string text)
	{
		var textMesh = GetComponent<TextMesh>();
		textMesh.text = text;
	}
}
