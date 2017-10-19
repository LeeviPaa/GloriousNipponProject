using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.PostProcessing;

public class PlayerVelocityBlackout : MonoBehaviour
{
    public VRTK_BodyPhysics bodyPhysics;
    public float minBlackoutVelocity;
    public float maxBlackoutVelocity;
    private PostProcessingBehaviour postProsessing;
    private float originalVignetteIntensity;

    void Awake()
    {
        if (!bodyPhysics)
        {
            bodyPhysics = GetComponent<VRTK_BodyPhysics>();
        }
        if (!bodyPhysics)
        {
            enabled = false;
        }
    }

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
            postProsessing = camera.GetComponent<PostProcessingBehaviour>();
            if (postProsessing)
            {
                originalVignetteIntensity = postProsessing.profile.vignette.settings.intensity;
            } 
        }     
    }

    void Update()
    {
        print(bodyPhysics.GetVelocity().magnitude);
        if (postProsessing)
        {
            VignetteModel.Settings settings = postProsessing.profile.vignette.settings;
            settings.intensity = Mathf.InverseLerp(minBlackoutVelocity, maxBlackoutVelocity, bodyPhysics.GetVelocity().magnitude);
            postProsessing.profile.vignette.settings = settings;
        }
    }

    void OnDestroy()
    {
        if (postProsessing)
        {
            VignetteModel.Settings settings = postProsessing.profile.vignette.settings;
            settings.intensity = originalVignetteIntensity;
            postProsessing.profile.vignette.settings = settings;
        }
    }
}
