﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualKeyBoard : MonoBehaviour
{
    public VirtualDisplay display;
    [SerializeField]
    GameObject keytop;
    private static string[] keyboardStr = {
        "1","!","2","\"","3","#","4","$","5","%","6","&","7","\'","8","(","9",")","0","0",
        "q","Q","w","W","e","E","r","R","t","T","y","Y","u","U","i","I","o","O","p","P",
        "a","A","s","S","d","D","f","F","g","G","h","H","j","J","k","K","l","L",
        "sft","SFT","z","Z","x","X","c","C","v","V","b","B","n","N","m","M",
        "BS","BS","space","space",
    };
    GameObject[] keys = new GameObject[39];

    bool shiftState;

    string output = string.Empty;
    // Use this for initialization
    void Start()
    {
        var margin = -0.26f;
        var adjust_x = -1.25f;
        float x, y;
        float vk_x = transform.position.x;
        float vk_y = transform.position.y;
        float vk_z = transform.position.z;
        Vector3 pos;
        // 1234567890
        for (int i = 0; i < 10; i++)
        {
            x = vk_x + adjust_x + 0.25f * i;
            pos = new Vector3(x, vk_y, vk_z);
            keys[i] = Instantiate(keytop, pos, Quaternion.identity, transform);
            keys[i].GetComponent<Keytop>().Type = Keytop.EKeyType.character;
        }
        //qwertyuiop
        for (int i = 0; i < 10; i++)
        {
            x = vk_x + adjust_x + 0.25f * i;
            y = vk_y + margin;
            pos = new Vector3(x, y, vk_z);
            keys[10 + i] = Instantiate(keytop, new Vector3(x, y, vk_z), Quaternion.identity, transform);
            keys[10 + i].GetComponent<Keytop>().Type = Keytop.EKeyType.character;

        }
        //asdfghjkl
        for (int i = 0; i < 9; i++)
        {
            x = vk_x + adjust_x + 0.125f + 0.25f * i;
            y = vk_y + margin * 2;
            pos = new Vector3(x, y, vk_z);
            keys[20 + i] = Instantiate(keytop, pos, Quaternion.identity, transform);
            keys[20 + i].GetComponent<Keytop>().Type = Keytop.EKeyType.character;

        }
        //shift
        x = vk_x + adjust_x + 0;
        y = vk_y + margin * 3;
        pos = new Vector3(x, y, vk_z);
        keys[29] = Instantiate(keytop, pos, Quaternion.identity, transform);
        keys[29].GetComponent<Keytop>().Type = Keytop.EKeyType.shift;

        var rect = keys[29].GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(41.0f, 30.0f);
        rect.anchoredPosition += new Vector2(15f, 0f);
        //zxcvbnm
        for (int i = 0; i < 7; i++)
        {
            x = vk_x + adjust_x + 0.375f + 0.25f * i;
            y = vk_y + margin * 3;
            pos = new Vector3(x, y, vk_z);
            keys[30 + i] = Instantiate(keytop, pos, Quaternion.identity, transform);
            keys[30 + i].GetComponent<Keytop>().Type = Keytop.EKeyType.character;

        }
        // back space 
        x = vk_x + keys[36].transform.position.x + 0.25f;
        y = vk_y + margin * 3;
        pos = new Vector3(x, y, vk_z);
        keys[37] = Instantiate(keytop, pos, Quaternion.identity, transform);
        keys[37].GetComponent<Keytop>().Type = Keytop.EKeyType.back_space;
        rect = keys[37].GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(41.0f, 30.0f);
        rect.anchoredPosition += new Vector2(14f, 0f);

        // space
        x = vk_x + adjust_x;
        y = vk_y + margin * 4;
        pos = new Vector3(x, y, vk_z);
        keys[38] = Instantiate(keytop, pos, Quaternion.identity, transform);
        keys[38].GetComponent<Keytop>().Type = Keytop.EKeyType.space;
        rect = keys[38].GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(130.0f, 30.0f);
        rect.anchoredPosition += new Vector2(249, 0f);
        rect = keys[38].transform.GetChild(0).GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(360.0f, 187.0f);


        //Default character is small.
        SetShiftKeyState(false);
    }
    public void SetShiftKeyState(bool value)
    {
        shiftState = value;
        var count = 0;

        if (value)
            foreach (var g in keys)
            {
                // Big character
                g.transform.GetComponent<Keytop>().SetKey(keyboardStr[count * 2 + 1]);
                count++;
            }
        else
            foreach (var g in keys)
            {
                // Small character
                g.transform.GetComponent<Keytop>().SetKey(keyboardStr[count * 2]);
                count++;
            }
    }
    public bool GetShiftState()
    {
        return shiftState;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
