using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

[RequireComponent(typeof(CanvasGroup))]
public class VRCanvas_Hand : VRCanvas
{
    [Tooltip("Angle after fading out is complete")]
    public float fadeCompleteAngleLimit;
    [Tooltip("Angle before fading out starts")]
    public float fadeStartAngleLimit;

    private Transform headset;
    private CanvasGroup canvasGroup;

    protected override void Start()
    {
        base.Start();

        canvasGroup = GetComponent<CanvasGroup>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        headset = VRTK_DeviceFinder.HeadsetCamera();
    }

    protected override void Update()
    {
        base.Update();

        if (headset)
        {
            float angle = Vector3.Angle(transform.forward, transform.position - headset.transform.position);
            float value = Mathf.InverseLerp(fadeCompleteAngleLimit, fadeStartAngleLimit, angle);
            canvasGroup.alpha = value;
            if (value < 0.1f)
            {
                canvasGroup.interactable = false;
            }
            else
            {
                canvasGroup.interactable = true;
            }
        }
    }
}
