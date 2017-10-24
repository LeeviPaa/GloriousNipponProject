using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRCanvasItem_DelayedButtonFillIcon : MonoBehaviour
{
    [SerializeField]
    private VRCanvasItem_DelayedButton button;
    [SerializeField]
    private Image fillImage;

    private void Start()
    {
        if (!button)
        {
            button = GetComponent<VRCanvasItem_DelayedButton>();
            if (!button)
            {
                enabled = false;
                return;
            }
        }
        if (!fillImage)
        {
            fillImage = GetComponent<Image>();
            if (!fillImage)
            {
                enabled = false;
                return;
            }
        }
    }

    private void Update()
    {
        fillImage.fillAmount = button.GetClickProgress();
    }
}
