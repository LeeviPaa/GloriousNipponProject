using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public abstract class LevelInstance : MonoBehaviour
{
    protected VRTK_HeadsetFade headsetFade;
	protected Color headsetFadeColor = Color.black;
	protected Transform playArea;
	protected GameObject playerScriptHolder;

	protected virtual void Awake()
    {
        VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        GameManager.levelInstance = this;
        GameManager.OnLevelAwake();
    }

    protected virtual void Start()
    {	
		GameManager.OnLevelStart();
    }

    protected virtual void Update()
    {
        GameManager.OnLevelUpdate();
    }

    public virtual void ScreenFade(float duration)
    {
        if (headsetFade)
            headsetFade.Fade(headsetFadeColor, duration);
    }

    public virtual void ScreenUnfade(float duration)
    {
        if (headsetFade)
            headsetFade.Unfade(duration);
    }

    protected virtual void OnEnable()
    {
		playArea = VRTK_DeviceFinder.PlayAreaTransform();
		playerScriptHolder = GameObject.FindGameObjectWithTag("PlayerScriptHolder");
		if (playerScriptHolder)
		{
			headsetFade = playerScriptHolder.GetComponent<VRTK_HeadsetFade>();
		}			
	}

    protected virtual void OnDisable()
    {

    }

    protected virtual void OnDestroy()
    {
        VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
    }
}
