﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEventRerouter : MonoBehaviour
{
    public void ChangeScene(string name)
    {
        GameManager.ChangeScene(name);
    }
}
