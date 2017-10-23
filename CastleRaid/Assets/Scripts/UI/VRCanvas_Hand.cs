using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

[RequireComponent(typeof(CanvasGroup))]
public class VRCanvas_Hand : MonoBehaviour
{
    [Tooltip("Angle after fading out is complete")]
    public float fadeCompleteAngleLimit;
    [Tooltip("Angle before fading out starts")]
    public float fadeStartAngleLimit;

    private Transform headset;
    private CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
    }

    protected virtual void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    protected virtual void OnEnable()
    {
        headset = VRTK_DeviceFinder.HeadsetCamera();
    }

    protected virtual void OnDisable()
    {
        
    }

    protected virtual void Update()
    {
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
