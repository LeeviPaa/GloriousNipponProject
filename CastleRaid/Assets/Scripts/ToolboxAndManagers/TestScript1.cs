using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class TestScript1 : MonoBehaviour
{
	public Transform playArea;

	[AddEditorInvokeButton]
    public void FadeTest()
    {
		VRTK_HeadsetFade fadez = playArea.GetComponent<VRTK_HeadsetFade>();
		fadez.Fade(Color.black, 5f);
    }
}
