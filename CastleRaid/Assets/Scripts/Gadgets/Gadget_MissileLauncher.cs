﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Gadget_MissileLauncher : MonoBehaviour
{
    [SerializeField]
    private VRTK_InteractableObject interaction;
    [SerializeField]
    private Transform targetPainter;
    [SerializeField]
    private Transform missileSpawnPoint;
    [SerializeField]
    private GameObject missilePrefab;
    [SerializeField]
    private VRCanvasItem_TargetLockIndicator indicator;
    [SerializeField]
    private float lockTimeRequired = 1f;

    private float lockProgress = 0f;
    private LockableTarget lockedTarget;

    void Start()
    {
        if (!interaction)
        {
            interaction = GetComponent<VRTK_InteractableObject>();
        }

        if (interaction)
        {
            interaction.InteractableObjectUsed += OnUseBegin;
            interaction.InteractableObjectUnused += OnUseEnd;
            interaction.InteractableObjectGrabbed += OnGrabBegin;
            interaction.InteractableObjectUngrabbed += OnGrabEnd;
        }
        else
        {
            enabled = false;
        }
    }

    void Update()
    {
        UpdateTargetLock();
    }

    void OnGrabBegin(object sender, InteractableObjectEventArgs e)
    {
        
    }

    void OnGrabEnd(object sender, InteractableObjectEventArgs e)
    {
        lockProgress = 0f;
    }

    void OnUseBegin(object sender, InteractableObjectEventArgs e)
    {
        if (lockProgress >= 0.95f)
        {
            Shoot();
        }
    }

    void OnUseEnd(object sender, InteractableObjectEventArgs e)
    {
        
    }

    void UpdateTargetLock()
    {
        if (interaction.IsGrabbed() && targetPainter)
        {
            RaycastHit hit;
            if (Physics.Raycast(targetPainter.position, targetPainter.forward, out hit))
            {
                lockedTarget = hit.transform.GetComponentInParent<LockableTarget>();
                if (lockedTarget)
                {
                    lockProgress = Mathf.Clamp01(lockProgress + Time.deltaTime / lockTimeRequired);
                }
            }
            if (!lockedTarget)
            {
                lockProgress = 0f;
            }
            if (indicator)
            {
                indicator.SetLockProgress(lockProgress);
            }
        }
    }

    void Shoot()
    {
        Gadget_Missile missile = Instantiate(missilePrefab, missileSpawnPoint.position, missileSpawnPoint.rotation).GetComponent<Gadget_Missile>();
        missile.Shoot(lockedTarget.transform);
    }
}
