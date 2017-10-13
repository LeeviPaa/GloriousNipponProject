using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.UI;

public class VRCanvas_DebugHUD : MonoBehaviour
{
    public Canvas canvas;
    public Text timerText;

    void Start()
    {
        if (canvas != null)
        {
            canvas.planeDistance = 0.5f;
        }

        Transform sdkCamera = VRTK_DeviceFinder.HeadsetCamera();
        if (sdkCamera != null)
        {
            canvas.worldCamera = sdkCamera.GetComponent<Camera>();
        }
    }

    void Update()
    {
        timerText.text = ((LevelInstance_Game)GameManager.levelInstance).levelTimeLeft.ToString();
    }
}
