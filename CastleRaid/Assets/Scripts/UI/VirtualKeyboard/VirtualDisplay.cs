using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualDisplay : MonoBehaviour
{
    Text txt;
    // Use this for initialization
    void Start()
    {
        txt = GetComponent<Text>();
        Clear();
    }

    // Update is called once per frame
    void Update()
    {

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
        txt.text.Remove(txt.text.Length - 1);
    }
}
