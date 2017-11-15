using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keytop : MonoBehaviour
{
    VirtualKeyBoard vkb;

    public enum EKeyType
    {
        none,
        character,
        shift,
        back_space,
        space,
        submit
    }
    private EKeyType type;
    public EKeyType Type
    {
        get { return type; }
        set
        {
            if (type == EKeyType.none)
                type = value;
            else
                Debug.LogError(value.ToString() + "  already exists. So you can not set again.");

            if (type == EKeyType.character)
                character = transform.GetChild(0).GetComponent<Text>().text[0];
        }
    }

    char character;

    void Start()
    {
        vkb = transform.parent.GetComponent<VirtualKeyBoard>();
        if (vkb == null)
            Debug.LogError("There is not virtual keyboard as parent");


    }
    public void PushKey()
    {
        string temp = string.Empty;
        switch (type)
        {
            case EKeyType.none:
                Debug.LogError("There is empty key");
                break;
            case EKeyType.character:              
                vkb.display.Output(ref temp);
                if (temp.Length < vkb.charLimit)
                {
                    vkb.display.Add(character);
                }
                break;
            case EKeyType.shift:
                vkb.SetShiftKeyState(!vkb.GetShiftState());
                break;
            case EKeyType.back_space:
                vkb.display.DeleteOneCharacter();
                break;
            case EKeyType.space:
                vkb.display.Add(' ');
                break;
            case EKeyType.submit:
                vkb.display.Output(ref temp);
                vkb.OnSubmit(temp);
                break;
            default:
                break;
        }
    }

    public void SetKey(char value)
    {
        character = value;
        transform.GetChild(0).GetComponent<Text>().text = value.ToString();
    }
    public void SetKey(string value)
    {
        character = value[0];
        transform.GetChild(0).GetComponent<Text>().text = value;
        name = ToString();
    }
    public override string ToString()
    {
        if (type == EKeyType.character)
            return "Key : " + character;
        else
            return "Key : " + type.ToString();
    }
}
