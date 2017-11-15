using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    [Tooltip("Objects that are the this object's children but will be reparented when the parent would be destroyed")]
    [SerializeField]
    private GameObject[] savedObjects;
    [SerializeField]
    private GameObject[] spawnedObjects;
    [SerializeField]
    private float objectSpawnRadius;
    [SerializeField]
    private string[] effects;

    public void Destroy()
    {
        foreach (GameObject obj in savedObjects)
        {
            obj.transform.SetParent(transform.parent, true);
        }
        foreach (GameObject obj in savedObjects)
        {
            Instantiate(obj, transform.position + Random.insideUnitSphere * objectSpawnRadius, Quaternion.identity);
        }
        foreach (string eff in effects)
        {
            GameManager.effectManager.GetEffect(eff, true, true, transform.position, transform.rotation);
        }       
        Destroy(gameObject);
    }
}
