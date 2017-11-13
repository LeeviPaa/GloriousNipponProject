using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Gadget_Gyrocopter : MonoBehaviour
{
    [SerializeField]
    Transform propellerTransform;
    VRTK_InteractableObject interactableScript;
    Rigidbody rb;
    bool isUsed = false;
    float liftForce = 800f;
    float rotateSpeed = 1080f;

    private void Awake()
    {
        interactableScript = GetComponent<VRTK_InteractableObject>();
        interactableScript.InteractableObjectUsed += OnUse;
        interactableScript.InteractableObjectUnused += OnUnuse;

        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isUsed)
        {
            Vector3 newPropellerRotation = Vector3.zero;
            newPropellerRotation.y = rotateSpeed * Time.deltaTime;
            propellerTransform.rotation *= Quaternion.Euler(newPropellerRotation);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //if (isUsed)
            //{
            //    OnUnuse(this, new InteractableObjectEventArgs());
            //}
            //else
            //{
            //    OnUse(this, new InteractableObjectEventArgs());
            //}

            OnUse(this, new InteractableObjectEventArgs());
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnUnuse(this, new InteractableObjectEventArgs());
        }
    }

    private void FixedUpdate()
    {
        if (isUsed)
        {
            rb.AddForce(transform.up * liftForce * Time.fixedDeltaTime);
        }
    }

    private void OnUse(object sender, InteractableObjectEventArgs e)
    {
        isUsed = true;
    }

    private void OnUnuse(object sender, InteractableObjectEventArgs e)
    {
        isUsed = false;
    }

}
