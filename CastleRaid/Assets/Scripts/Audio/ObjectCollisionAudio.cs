using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollisionAudio : MonoBehaviour
{
    [SerializeField]
    private string audioName;
    [SerializeField]
    private float audioBlockTime;
    [SerializeField]
    private float minimumReqVelocity;

    private bool audioPlayable = true;

    void OnCollisionEnter(Collision col)
    {
        if (audioPlayable && col.relativeVelocity.magnitude >= minimumReqVelocity)
        {
            GameManager.audioManager.GetAudio(audioName, true, true, col.contacts[0].point, transform);
            audioPlayable = false;
            StartCoroutine(ClearAudioPlayBlock());
        }       
    }

    IEnumerator ClearAudioPlayBlock()
    {
        yield return new WaitForSeconds(audioBlockTime);
        audioPlayable = true;
    }
}
