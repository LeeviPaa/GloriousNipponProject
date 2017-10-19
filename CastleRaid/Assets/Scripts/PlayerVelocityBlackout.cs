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
    public float maxDelta;
    [Range(0f,1f)]
    public float maxFadeIntensity;

    private VignetteModel vignette;
    private float originalVignetteIntensity;

    private Vector3 lastBodyPos;
    private float lastBodyVelocity = 0f;
    private float lastBodyAcceleration = 0f;
    private Transform playArea;

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
        if (playArea)
        {
            Vector3 pos = playArea.position;
            float velocity = (pos - lastBodyPos).magnitude / Time.deltaTime;
            float acceleration = velocity - lastBodyVelocity;
            if (vignette != null)
            {
                VignetteModel.Settings settings = vignette.settings;
                switch (detectionMode)
                {
                    case Mode.Velocity:
                        settings.intensity =
                            Mathf.Lerp(0, maxFadeIntensity,
                            Mathf.InverseLerp(startLimit, endLimit, 
                            Mathf.MoveTowards(lastBodyVelocity, velocity, maxDelta)));
                        break;

                    case Mode.Acceleration:
                        settings.intensity =
                            Mathf.Lerp(0, maxFadeIntensity,
                            Mathf.InverseLerp(startLimit, endLimit,
                            Mathf.MoveTowards(lastBodyAcceleration, acceleration, maxDelta)));
                        break;

                    default:
                        break;
                }
                vignette.settings = settings;
            }
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
