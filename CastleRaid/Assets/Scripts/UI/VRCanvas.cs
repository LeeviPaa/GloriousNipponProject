using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.UI;

public abstract class VRCanvas : MonoBehaviour
{
    public Canvas canvas;
    public VRTK_UICanvas canvasInteraction;

    protected virtual void Awake()
    {
        VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void OnEnable()
    {
        if (!canvas)
        {
            canvas = GetComponent<Canvas>();
        }
        if (!canvas)
        {
            canvasInteraction = GetComponent<VRTK_UICanvas>();
        }
    }

    protected virtual void OnDisable()
    {

    }

    protected virtual void Update()
    {

    }

    protected virtual void OnDestroy()
    {
        VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
    }
}
