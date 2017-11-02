using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderController : MonoBehaviour
{
    public delegate void ColliderControllerColliderVoid(ColliderController colliderController, Collider collider);
    public event ColliderControllerColliderVoid _OnTriggerEnter;
    public event ColliderControllerColliderVoid _OnTriggerExit;


    private void OnTriggerEnter(Collider other)
    {
		if(_OnTriggerEnter != null)
		{
			_OnTriggerEnter(this, other);
		}
    }

    private void OnTriggerExit(Collider other)
	{
		if (_OnTriggerExit != null)
		{
			_OnTriggerExit(this, other);
		}
    }
}
