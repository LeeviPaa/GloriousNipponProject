using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.UI;

public class VRCanvas_DebugHUD : VRCanvas
{
    public Text timerText;

    protected override void Start()
    {
        base.Start();

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

    protected override void Update()
    {
        base.Update();

        timerText.text = ((LevelInstance_Game)GameManager.levelInstance).levelTimeLeft.ToString();
    }
}
