﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    [SerializeField]
    float guardingDuration = 0;

    public float GetGuardingDuration()
    {
        return guardingDuration;
    }
}
