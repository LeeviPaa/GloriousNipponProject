using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRCanvas_MainMenu : MonoBehaviour
{
    public void ButtonChangeScene(string name)
    {
        GameManager.ChangeScene(name);
    }
}
