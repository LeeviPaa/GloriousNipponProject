using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRDebug : MonoBehaviour
{
    [SerializeField]
    private bool debugActive = true;
    [SerializeField]
	private GameObject fpsCanvasPrefab;

	private GameObject fpsCanvas;

	public void ToggleFPSCanvas(bool state) 
	{
		if (state && !fpsCanvas) 
		{
			fpsCanvas = Instantiate(fpsCanvasPrefab, Vector3.zero, Quaternion.identity);
			Canvas canvas = fpsCanvas.GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceCamera;
			canvas.worldCamera = VRTK.VRTK_DeviceFinder.HeadsetCamera().GetComponent<Camera>();
            Debug.Log("FPS counter created for debug");
		}
		if (!state && fpsCanvas)
		{
			Destroy(fpsCanvas);
            Debug.Log("FPS counter removed for debug");
        }
	}

    public void ToggleUnlimitedTime(bool state)
    {
        LevelInstance_Game li = GameManager.levelInstance as LevelInstance_Game;
        if (li)
        {
            
            li.SetTimerActive(state);
            if (state)
            {
                Debug.Log("Level timer paused for debug");
            }
            else
            {
                Debug.Log("Level timer started for debug");
            }
        }
    }
}
