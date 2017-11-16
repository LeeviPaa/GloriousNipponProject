using System.Collections;
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
    [SerializeField]
    private float lifetime = 10f;

    private Transform target;
	private GameObject[] ignore;
    private float lifetimeTimer = 0f;

    public void Shoot(Transform target, GameObject[] ignore)
    {
		this.target = target;
		this.ignore = ignore;
        lifetimeTimer = 0f;
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
        else
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        if (lifetimeTimer >= lifetime)
        {
            Explode();
        }
	}

	void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
			for (int i = 0; i < ignore.Length; i++)
			{
				if (other.gameObject == ignore[i])
				{
					return;
				}

				Transform[] parentTransforms = other.gameObject.GetComponentsInParent<Transform>();

				foreach (Transform t in parentTransforms)
				{
					if (t.gameObject == ignore[i])
					{
						return;
					}
				}
			}

			Debug.Log("Exploding on touch with non-ignored objects!");
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
