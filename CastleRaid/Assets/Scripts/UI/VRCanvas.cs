using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.UI;

public abstract class VRCanvas : MonoBehaviour
{
    public Canvas canvas;
    public VRTK_UICanvas canvasInteraction;

    [SerializeField]
    private VRCanvasWindow currentWindow;

    protected virtual void Awake()
    {
        VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
    }

    protected virtual void Start()
    {
        if (currentWindow)
        {
            currentWindow.SetState(true);
        }
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

    public void ChangeActiveWindow(VRCanvasWindow newWindow)
    {
        if (currentWindow)
        {
            currentWindow.SetState(false);
        }
        if (newWindow)
        {
            newWindow.SetState(true);
        }
    }
}
