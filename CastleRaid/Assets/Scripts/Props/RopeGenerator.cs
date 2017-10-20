using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RopeGenerator : MonoBehaviour {

    public Transform RopeStartBone;
    public Transform RopeEndBone;
    public Transform startPos;
    private Vector3 startPrevPos = Vector3.zero;
    public Transform endPos;
    private Vector3 endPrevPos = Vector3.zero;
    private void Start()
    {

    }

    private void Update()
    {
        if (startPrevPos != startPos.position || endPrevPos != endPos.position)
        {
            
            if (RopeStartBone != null && RopeEndBone != null)
            {
                if (startPos != null && endPos != null)
                {
                    RopeStartBone.position = startPos.position;
                    RopeEndBone.position = endPos.position;
                }
                else
                {
                    RopeStartBone.position = Vector3.zero;
                    RopeEndBone.position = Vector3.zero;
                }
            }
        }

        startPrevPos = startPos.position;
        endPrevPos = endPos.position;
    }
}
