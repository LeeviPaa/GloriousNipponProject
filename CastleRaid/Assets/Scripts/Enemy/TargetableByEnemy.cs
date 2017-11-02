using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetableByEnemy : MonoBehaviour
{
    //MainObject should be the main gameObject of a single actor
    //For example, if there are separate colliders on player's hands and torso, the mainObject would be the player root object.
    [SerializeField]
    GameObject mainObject;

    public GameObject GetMainObject()
    {
        if(mainObject == null)
        {
            mainObject = gameObject;
            Debug.LogError("MainObject was null, this object set to mainObject");
        }

        return mainObject;
    }
}
