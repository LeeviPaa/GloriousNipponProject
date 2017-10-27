using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRCanvasItem_TimeIndicator : MonoBehaviour
{
    public Image timeIcon;

    private LevelInstance_Game levelInstance;

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
	}

    void Update()
    {
        timeIcon.fillAmount = levelInstance.levelTimeLeft / levelInstance.levelTimeLimit;
    }
}
