using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class VRCanvasItem_TimeIndicator : MonoBehaviour
{
    public Image timeIcon;
    public bool useTickAudio;
    public string tickAudioName;
    public float tickAudioIntensifyTimeStart;
    public float tickAudioIntensifyTimeEnd;
    public float tickAudioIntensifyVolume;
	public float tickAudioIntensifyPitch;

	private LevelInstance_Game levelInstance;
    private AudioItem tickAudio;
    private float tickAudioInitialVolume;
	private float tickAudioInitialPitch;
    private bool tickAudioIsIntensifying = false;

	void Awake()
	{
		VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
	}

	void Start()
	{

	}

	void OnEnable()
    {
        if (!timeIcon)
        {
            timeIcon = GetComponent<Image>();
            if (!timeIcon)
            {
                enabled = false;
                return;
            }
        }
        levelInstance = ((LevelInstance_Game)GameManager.levelInstance);
		if (!levelInstance)
		{
				enabled = false;
				return;
		}

        if (useTickAudio)
        {
			if (!tickAudio)
			{
				tickAudio = GameManager.audioManager.GetAudio(tickAudioName, false, false, transform.position, transform);
			}

			if (!tickAudio)
            {
                useTickAudio = false;
            }
            else
            {
                tickAudioInitialVolume = tickAudio.source.volume;
				tickAudioInitialPitch = tickAudio.source.pitch;
				tickAudio.source.Play();
			}
        }
    }

    void Update()
    {
        timeIcon.fillAmount = levelInstance.levelTimeLeft / levelInstance.levelTimeLimit;

        if (useTickAudio && tickAudio)
        {
            if (!tickAudioIsIntensifying && levelInstance.levelTimeLeft <= tickAudioIntensifyTimeStart)
            {
                tickAudioIsIntensifying = true;
            }

            if (tickAudioIsIntensifying)
            {
                if (levelInstance.levelTimeLeft <= tickAudioIntensifyTimeEnd)
                {
                    tickAudioIsIntensifying = false;
                }
				float t = Mathf.InverseLerp(tickAudioIntensifyTimeStart, tickAudioIntensifyTimeEnd, levelInstance.levelTimeLeft);
				tickAudio.source.volume = Mathf.Lerp(tickAudioInitialVolume, tickAudioIntensifyVolume, t);
				tickAudio.source.pitch = Mathf.Lerp(tickAudioInitialPitch, tickAudioIntensifyPitch, t);
			}
        }
    }

	void OnDestroy()
	{
		VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
	}
}
