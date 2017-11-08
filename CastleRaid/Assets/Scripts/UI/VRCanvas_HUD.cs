using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.UI;

public class VRCanvas_HUD : VRCanvas
{
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private LootBagController lootbag;

    protected override void Start()
    {
        base.Start();

        //lootbag = GameManager.levelInstance.GetPlayerScriptHolder().GetComponentInChildren<LootBagController>();
        lootbag.OnLootTotalValueChange += ScoreChanged;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Transform sdkCamera = VRTK_DeviceFinder.HeadsetCamera();
        if (sdkCamera)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.planeDistance = 0.5f;
            canvas.worldCamera = sdkCamera.GetComponent<Camera>();
        }
        else
        {
            enabled = false;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        lootbag.OnLootTotalValueChange -= ScoreChanged;
    }

    void ScoreChanged(int value)
    {
        scoreText.text = value.ToString();
    }
}
