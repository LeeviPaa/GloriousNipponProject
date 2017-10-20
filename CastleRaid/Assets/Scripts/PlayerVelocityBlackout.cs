using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.PostProcessing;

public class PlayerVelocityBlackout : MonoBehaviour
{
    public enum Mode { Velocity, Acceleration }
    public Mode detectionMode;
    public float startLimit;
    public float endLimit;
    public float dampTime;
    public float maxDampDelta;
    [Range(0f,1f)]
    public float maxFadeIntensity;

    private VignetteModel vignette;
    private float originalVignetteIntensity;

    private Vector3 lastBodyPos;
    private float lastBodyVelocity = 0f;
    private float lastBodyAcceleration = 0f;
    private float currentDeltaVelocity = 0f;
    private float currentValue = 0f;
    private Transform playArea;
    private float currentDelta = 0f;

    void Start()
    {
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return null;
        Transform camera = VRTK_DeviceFinder.HeadsetCamera();
        if (camera)
        {
            PostProcessingBehaviour pp = camera.GetComponent<PostProcessingBehaviour>();
            if (pp && pp.profile)
            {
                vignette = pp.profile.vignette;
                originalVignetteIntensity = vignette.settings.intensity;
            } 
        }
        playArea = VRTK_DeviceFinder.PlayAreaTransform();
        if (playArea)
        {
            lastBodyPos = playArea.position;
        }
    }

    void Update()
    {
        if (playArea && vignette != null)
        {
            Vector3 pos = playArea.position;
            float velocity = (pos - lastBodyPos).magnitude / Time.deltaTime;
            float acceleration = velocity - lastBodyVelocity;
            float target = 0f;
            VignetteModel.Settings newSettings = vignette.settings;
            switch (detectionMode)
            {
                case Mode.Velocity:
                    target = Mathf.InverseLerp(startLimit, endLimit, velocity);
                    break;

                case Mode.Acceleration:
                    target = Mathf.InverseLerp(startLimit, endLimit, acceleration);
                    break;

                default:
                    break;
            }
            currentValue = Mathf.SmoothDamp(currentValue, target, ref currentDeltaVelocity, dampTime, maxDampDelta);
            newSettings.intensity = Mathf.Lerp(0, maxFadeIntensity, currentValue);
            vignette.settings = newSettings;
            lastBodyPos = pos;
            lastBodyVelocity = velocity;
            lastBodyAcceleration = acceleration;
        }
    }

    void OnDestroy()
    {
        if (vignette != null)
        {
            VignetteModel.Settings settings = vignette.settings;
            settings.intensity = originalVignetteIntensity;
            vignette.settings = settings;
        }
    }
}
