using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualDisplay : MonoBehaviour
{
    Text txt;

    void Start()
    {
        txt = GetComponent<Text>();
        Clear();
    }

    public void Add(string value)
    {
        txt.text += value;
    }
    public void Add(char value)
    {
        txt.text += value;
    }
    public void Clear()
    {
        txt.text = string.Empty;
    }
    public void DeleteOneCharacter()
    {
        if (txt.text.Length > 0)
            txt.text = txt.text.Remove(txt.text.Length - 1, 1);
    }

    public void Output(ref string reference)
    {
        reference = txt.text;
    }
}
