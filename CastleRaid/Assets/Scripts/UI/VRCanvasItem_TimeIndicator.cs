using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRCanvasItem_TimeIndicator : MonoBehaviour
{
    public Image timeIcon;
    public bool useTickAudio;
    public string tickAudioName;
    public float tickAudioIntensifyTimeStart;
    public float tickAudioIntensifyTimeEnd;
    public float tickAudioIntensifyVolume;

    private LevelInstance_Game levelInstance;
    private AudioItem tickAudio;
    private float tickAudioInitialVolume;
    private bool tickAudioIsIntensifying = false;

    void Start()
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
            tickAudio = GameManager.audioManager.GetAudio(tickAudioName, true, false, transform.position, transform);
            if (!tickAudio)
            {
                useTickAudio = false;
            }
            else
            {
                tickAudioInitialVolume = tickAudio.source.volume;
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
                tickAudio.source.volume =
                    Mathf.Lerp(tickAudioInitialVolume, tickAudioIntensifyVolume,
                    Mathf.InverseLerp(tickAudioIntensifyTimeStart, tickAudioIntensifyTimeEnd, levelInstance.levelTimeLeft));
            }
        }
    }
}
