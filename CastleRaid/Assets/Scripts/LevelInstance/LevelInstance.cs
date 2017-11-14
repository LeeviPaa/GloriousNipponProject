using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public abstract class LevelInstance : MonoBehaviour
{
	protected GameObject playerScriptHolder;
    protected PlayerRelocator playerRelocator;
    protected VRTK_HeadsetFade headsetFade;
    protected Color headsetFadeColor = Color.black;
    [SerializeField]
    protected bool useBgmAudio;
    [SerializeField]
    protected string bgmAudioName;

    private AudioItem bgmAudio;

    protected virtual void Awake()
    {
        VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        GameManager.levelInstance = this;
        playerScriptHolder = GameObject.FindGameObjectWithTag("PlayerScriptHolder");
        if (playerScriptHolder)
        {
            headsetFade = playerScriptHolder.GetComponent<VRTK_HeadsetFade>();
            playerRelocator = playerScriptHolder.GetComponent<PlayerRelocator>();
        }
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
		print(headsetFade + " fade");
        if (headsetFade)
            headsetFade.Fade(headsetFadeColor, duration);
    }

    public virtual void ScreenUnfade(float duration)
    {
		print(headsetFade + " unfade");
		if (headsetFade)
            headsetFade.Unfade(duration);
    }

    protected virtual void InitBgmAudio()
    {
        if (useBgmAudio)
        {
            bgmAudio = GameManager.audioManager.GetAudio(bgmAudioName, true, false, Vector3.zero, transform);
            if (!bgmAudio)
            {
                useBgmAudio = false;
            }
        }
    }

    protected virtual void OnEnable()
    {
				
	}

    protected virtual void OnDisable()
    {

    }

    protected virtual void OnDestroy()
    {
        VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
    }

    public GameObject GetPlayerScriptHolder()
    {
        return playerScriptHolder;
    }

    public PlayerRelocator GetPlayerRelocator()
    {
        return playerRelocator;
    }
}
