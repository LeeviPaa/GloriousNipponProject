using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class VRCanvasItem_DelayedButton : Selectable
{
    public UnityEvent OnDelayedClick;

    [SerializeField]
    protected float clickTimeNeeded;
    protected float clickTimer = 0f;
    protected bool buttonPressedState = false;

    protected override void Start()
    {
        base.Start();

        ResetClickTimer();
    }

    public override void OnPointerDown(PointerEventData e)
    {
        base.OnPointerDown(e);
        buttonPressedState = true;
    }

    public override void OnPointerUp(PointerEventData e)
    {
        base.OnPointerUp(e);

        buttonPressedState = false;
        ResetClickTimer();
    }

    protected virtual void ResetClickTimer()
    {
        clickTimer = clickTimeNeeded;
    }

    protected virtual void Update()
    {
        if (buttonPressedState)
        {
            clickTimer -= Time.deltaTime;
            if (clickTimer <= 0f)
            {
                clickTimer = 0f;
                buttonPressedState = false;
                OnDelayedClick.Invoke();
            }
        }
    }

    public float GetClickProgress()
    {
        return 1f - (clickTimer / clickTimeNeeded);
    }
}
