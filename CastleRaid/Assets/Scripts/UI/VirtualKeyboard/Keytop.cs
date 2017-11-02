using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keytop : MonoBehaviour
{
    public void PushKey()
    {
        print(transform.GetChild(0).GetComponent<Text>().text);
    }
}
