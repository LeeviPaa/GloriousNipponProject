using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distractor : MonoBehaviour
{
    [SerializeField]
    float range = 10f;
    [SerializeField]
    float distractDuration = 1f;
    [SerializeField]
    Collider areaTrigger;
    bool isActive = false;
    float distractTimer = -1;

    private void Start()
    {
        isActive = false;
        areaTrigger.enabled = false;
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            distractTimer -= Time.fixedDeltaTime;

            if(distractTimer <= 0)
            {
                isActive = false;
                areaTrigger.enabled = false;
            }
        }
    }

    public void Distract()
    {
        areaTrigger.enabled = true;
        distractTimer = distractDuration;
        isActive = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyPatroller enemy = other.GetComponent<EnemyPatroller>();

        if(enemy != null)
        {
            enemy.Distract(transform.position);
        }
    }
}
