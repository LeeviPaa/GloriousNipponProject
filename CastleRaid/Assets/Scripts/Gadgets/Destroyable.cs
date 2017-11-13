using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    [Tooltip("Objects that are the this object's children but will be reparented when the parent would be destroyed")]
    [SerializeField]
    private GameObject[] savedObjects;

    public void Destroy()
    {
        foreach (GameObject obj in savedObjects)
        {
            obj.transform.SetParent(transform.parent, true);
        }
        Destroy(gameObject);
    }
}
