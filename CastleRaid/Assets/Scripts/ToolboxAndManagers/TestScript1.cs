using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript1 : MonoBehaviour
{
    [AddEditorInvokeButton]
    public void FadeTest()
    {
        VRTK.VRTK_HeadsetFade fadez = VRTK.VRTK_DeviceFinder.PlayAreaTransform().GetComponent<VRTK.VRTK_HeadsetFade>();
        fadez.Fade(Color.black, 5f);
    }
}
