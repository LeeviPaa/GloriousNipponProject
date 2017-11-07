using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRDebug : MonoBehaviour
{
	[SerializeField]
	private GameObject fpsCanvasPrefab;

	private GameObject fpsCanvas;

	public void ToggleFPSCanvas() 
	{
		if (!fpsCanvas) 
		{
			fpsCanvas = Instantiate(fpsCanvasPrefab, Vector3.zero, Quaternion.identity);
			Canvas canvas = fpsCanvas.GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceCamera;
			canvas.worldCamera = VRTK.VRTK_DeviceFinder.HeadsetCamera().GetComponent<Camera>();
		}
		else
		{
			Destroy(fpsCanvas);
		}
	}
}
