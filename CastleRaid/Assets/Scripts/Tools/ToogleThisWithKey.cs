using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ObjectKeyPair
{
    public GameObject toggleObject;
    public KeyCode toggleKey;
    private bool onOff;
    public bool Toggle()
    {
        onOff = !onOff;
        return onOff;
    }
}

//[ExecuteInEditMode]
public class ToogleThisWithKey : MonoBehaviour {

    public List<ObjectKeyPair> L_ObjectKeyPair;

    private void OnSceneGUI()
    {
        foreach(ObjectKeyPair OKP in L_ObjectKeyPair)
        {
            if (OKP.toggleObject != null && OKP.toggleKey != null)
            {
                Event e = Event.current;
                switch (Event.current.type)
                {
                    case EventType.KeyDown:
                        {
                            if (Event.current.keyCode == OKP.toggleKey)
                            {
                                OKP.toggleObject.SetActive(OKP.Toggle());
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
            }
            }
        }
    }

}
