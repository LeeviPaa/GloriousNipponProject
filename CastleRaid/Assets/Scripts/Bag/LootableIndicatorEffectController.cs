using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class LootableIndicatorEffectController : MonoBehaviour
{
    VRTK_InteractableObject interactableObjectScript;
    ParticleSystem effect;

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

        effect = GetComponent<ParticleSystem>();
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
        if(effect != null)
        {
            effect.Stop();
        }
    }

    void OnEndGrab(object sender, InteractableObjectEventArgs e)
    {
        if (effect != null)
        {
            effect.Play();
        }
    }
}
