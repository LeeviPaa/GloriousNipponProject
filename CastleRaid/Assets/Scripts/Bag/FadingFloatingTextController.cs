using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingFloatingTextController : MonoBehaviour
{
    TextMesh textMesh;

    [SerializeField]
    float fadingDelta;
    float finalFadingDelta;
    [SerializeField]
    float fadingDeltaOverTimeMultiplier = 1;
    [SerializeField]
    float floatingDelta;
    float finalFloatingDelta;
    [SerializeField]
    float floatingDeltaOverTimeMultiplier = 1;
    float endAlphaThreshold = 0;
    bool active = false;
    Vector3 originalPosition;
    Transform mainCameraTransform;
    [SerializeField]
    Transform textParent;

    private void OnEnable()
    {
        active = false;
        originalPosition = transform.localPosition;
        mainCameraTransform = VRTK.VRTK_DeviceFinder.HeadsetCamera();
    }

    //[AddEditorInvokeButton]
    //void TestInitializator()
    //{
    //    InitializeText("Test");
    //}

    public void InitializeText(string textToDisplay)
    {
        textMesh = textParent.GetComponentInChildren<TextMesh>();

        if (textMesh)
        {
            textMesh.text = textToDisplay;
        }
        else
        {
            Debug.LogError("No textMesh component found in object or child objects!");
        }

        finalFadingDelta = fadingDelta;
        finalFloatingDelta = floatingDelta;
        transform.localPosition = originalPosition;
        Color originalColor = textMesh.color;
        originalColor.a = 1;
        textMesh.color = originalColor;

        active = true;
    }

    private void FixedUpdate()
    {
        if (active)
        {
            transform.localPosition += transform.up * finalFloatingDelta * Time.fixedDeltaTime;

            Color newColor = textMesh.color;
            newColor.a -= finalFadingDelta * Time.fixedDeltaTime;
            textMesh.color = newColor;

            if(newColor.a <= endAlphaThreshold)
            {
                active = false;
            }
            
            finalFadingDelta *= fadingDeltaOverTimeMultiplier;
            finalFloatingDelta *= floatingDeltaOverTimeMultiplier;


            if(mainCameraTransform == null)
            {
                mainCameraTransform = VRTK.VRTK_DeviceFinder.HeadsetCamera();
            }

            if (mainCameraTransform != null)
            {
                textParent.LookAt(mainCameraTransform);
            }

        }
    }

}
