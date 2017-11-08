using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRCanvasWindow : MonoBehaviour
{
    public void SetState(bool open)
    {
        gameObject.SetActive(open);
    }
}
