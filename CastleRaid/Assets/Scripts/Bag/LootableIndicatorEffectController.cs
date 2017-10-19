using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class LootableIndicatorEffectController : MonoBehaviour
{
    VRTK_InteractableObject interactableObjectScript;
    ParticleSystem[] effects;

    private void OnEnable()
    {
		Transform _parent = transform.parent;
		if (_parent)
		{
			interactableObjectScript = _parent.GetComponent<VRTK_InteractableObject>();
			if (interactableObjectScript != null)
			{
				interactableObjectScript.InteractableObjectGrabbed -= OnStartGrab;
				interactableObjectScript.InteractableObjectGrabbed += OnStartGrab;
				interactableObjectScript.InteractableObjectUngrabbed -= OnEndGrab;
				interactableObjectScript.InteractableObjectUngrabbed += OnEndGrab;
			}
		}

		effects = GetComponentsInChildren<ParticleSystem>();
    }

    private void OnDisable()
    {
        if (interactableObjectScript != null)
        {
            interactableObjectScript.InteractableObjectGrabbed -= OnStartGrab;
            interactableObjectScript.InteractableObjectUngrabbed -= OnEndGrab;
        }
    }

    void OnStartGrab(object sender, InteractableObjectEventArgs e)
	{
		if (effects != null)
        {
			int count = effects.Length;
			for (int i = 0; i < count; i++)
			{
				effects[i].Stop();
			}
        }
    }

    void OnEndGrab(object sender, InteractableObjectEventArgs e)
	{
		if (effects != null)
		{
			int count = effects.Length;
			for (int i = 0; i < count; i++)
			{
				effects[i].Play();
			}
        }
    }
}
