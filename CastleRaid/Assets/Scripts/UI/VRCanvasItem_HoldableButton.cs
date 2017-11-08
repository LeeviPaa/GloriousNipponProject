using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class VRCanvasItem_HoldableButton : Selectable
{
    public UnityEvent OnButtonHolded;

    protected virtual void Update()
    {
        if (IsPressed())
        {
            OnButtonHolded.Invoke();
        }
    }
}
