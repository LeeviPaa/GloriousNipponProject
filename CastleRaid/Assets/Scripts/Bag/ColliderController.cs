using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderController : MonoBehaviour
{
    public delegate void ColliderControllerColliderVoid(ColliderController coliderController, Collider collider);
    public event ColliderControllerColliderVoid _OnTriggerEnter;
    public event ColliderControllerColliderVoid _OnTriggerExit;


    private void OnTriggerEnter(Collider other)
    {
        _OnTriggerEnter(this, other);
    }

    private void OnTriggerExit(Collider other)
    {
        _OnTriggerExit(this, other);
    }
}
