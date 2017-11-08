using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRCanvasItem_ToggleEvent : MonoBehaviour
{
    public UnityEvent OnToggleOn;
    public UnityEvent OnToggleOff;

    [SerializeField]
    private bool state = false;
    [SerializeField]
    private bool initializeState = false;

    void Start()
    {
        if (initializeState)
        {
            if (!state)
            {
                OnToggleOff.Invoke();
            }
            else
            {
                OnToggleOn.Invoke();
            }
        }
    }

    public void Toggle()
    {
        if (state)
        {
            state = false;
            OnToggleOff.Invoke();
        }
        else
        {
            state = true;
            OnToggleOn.Invoke();
        }
    }

    public bool GetState()
    {
        return state;
    }
}
