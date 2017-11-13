﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadget_Missile : MonoBehaviour
{
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private float turnRate = 40f;
    [SerializeField]
    private float explosionRadius = 2f;

	private Transform target;

    public void Shoot(Transform target)
    {
		this.target = target;

    }

	void Update()
	{
		if (target)
		{
			Vector3 dir = target.position - transform.position;
			Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
			Quaternion actualRot = Quaternion.RotateTowards(transform.rotation, targetRot, turnRate);
			transform.rotation = actualRot;
			transform.position += transform.forward * speed * Time.deltaTime;
		}
	}

	void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            Explode();
        }
    }

    public void Explode()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, explosionRadius);
        List<Destroyable> validHit = new List<Destroyable>();
        foreach (Collider col in hit)
        {
            Destroyable target = col.transform.GetComponentInParent<Destroyable>();
            if (target)
            {
                validHit.Add(target);
            }
        }
        foreach (Destroyable target in validHit)
        {
            target.Destroy();
        }
        Destroy(gameObject);
    }
}
