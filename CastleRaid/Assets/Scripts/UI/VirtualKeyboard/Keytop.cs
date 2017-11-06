using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keytop : MonoBehaviour
{
    VirtualKeyBoard vkb;

    public enum EKeyType
    {
        character,
        shift,
        back_space,
        space,
        none
    }
    public EKeyType? type
    {
        get
        {
            return type.Value;
        }
        set
        {
            if (type.HasValue == false)
                type = value ;
            else
                Debug.LogError("You can not set again.");

        }
    }

    void Start()
    {
        vkb = transform.parent.GetComponent<VirtualKeyBoard>();
        if (vkb == null)
            Debug.LogError("There is not virtual keyboard as parent");
    }
    public void PushKey()
    {
        print(transform.GetChild(0).GetComponent<Text>().text);
    }
}
