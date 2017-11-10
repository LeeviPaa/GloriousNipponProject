using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineMover : MonoBehaviour
{
    [SerializeField]
    private BezierSpline targetSpline;
    [SerializeField]
    private bool randomizeStartPos;
    [SerializeField]
    [Range(0f,1f)]
    private float t = 0f;
    [SerializeField]
    private float speed = 0.2f;

    void Start()
    {
        if (randomizeStartPos)
        {
            t = Random.value;
        }
    }

    void Update()
    {
        t += speed * Time.deltaTime;
        if (t > 1f)
        {
            t -= 1f;
        }
        else if (t < 0f)
        {
            t += 1f;
        }
        if (targetSpline)
        {
            Vector3 pos = targetSpline.GetPoint(t);
            Vector3 dir = targetSpline.GetDirection(t);
            transform.position = pos;
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }
    [AddEditorInvokeButton]
    void VisualizePosition()
    {
        Vector3 pos = targetSpline.GetPoint(t);
        Vector3 dir = targetSpline.GetDirection(t);
        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }
}
