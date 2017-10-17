using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bag : VRTK.VRTK_InteractableObject
{

    enum EBagState
    {
        //Placed,
        Grabed,
        Carried
    }
    EBagState state = EBagState.Carried;

    [SerializeField]
    private BoxCollider col;
    [SerializeField]
    private AudioSource sound;
    [SerializeField]
    private UnityEngine.UI.Text txtBelongings;

    GameObject followObject;

    private Dictionary<TestGrabbableObject.EItemID, int> belongings = new Dictionary<TestGrabbableObject.EItemID, int>();

    TestGrabbableObject obj;
    TestGrabbableObject puttingObject;

    // Use this for initialization
    void Start()
    {
        //followObject = Camera.main.gameObject;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();


        // For camera
        //transform.position = Camera.main.transform.position - Camera.main.transform.forward;
        //transform.forward = Camera.main.transform.forward;

        // For hands.
        //if (followObject.GetComponent<Camera>())
        //{
        //    transform.position = Camera.main.transform.position - Camera.main.transform.forward;
        //    transform.localRotation = Camera.main.transform.localRotation;
        //    transform.forward = Camera.main.transform.forward;
        //}
        //else
        //{
        //    transform.position = followObject.transform.position;
        //    transform.localRotation = followObject.transform.localRotation;
        //}


    }

    public void SetFollowObject(GameObject obj)
    {
        followObject = obj;
    }

    float CheckDistance()
    {
        float result = Vector3.Distance(GameObject.FindWithTag("Player").transform.position, transform.position);
        print(result);
        return result;
    }

    private void OnTriggerEnter(Collider other)
    {
        var temp = other.GetComponent<TestGrabbableObject>();
        if (temp)
        {
            if (temp.IsGrabbed())
            {
                print("on trigger enter ");
                obj = temp;
            }
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (obj)
        {
            if (!obj.IsGrabbed() && obj.enabled)
            {
                print("on trigger stay");
                puttingObject = obj;
                PutTresure();
                obj.enabled = false;
            }
        }
    }


    void PutTresure()
    {
        print("PuttingToBag!!");

        IncreaseItemInBag();

        if (puttingObject.clip)
            sound.PlayOneShot(puttingObject.clip);
        else
            sound.PlayOneShot(sound.clip);


        //In future, chenge to animation .
        //Destroy(grabbingObject.gameObject);
        iTween.MoveTo(puttingObject.gameObject, iTween.Hash("position", transform.position, "time", 1.0f));
        iTween.ScaleTo(puttingObject.gameObject, iTween.Hash("time", 1.0f, "x", 0, "y", 0, "z", 0, "oncompletetarget", puttingObject.gameObject, "oncomplete", "DestroyPuttingObject"));

    }

    void DestroyPuttingObject()
    {
        Destroy(puttingObject.gameObject);
    }


    void IncreaseItemInBag()
    {
        if (belongings.ContainsKey(puttingObject.id))
            belongings[puttingObject.id]++;
        else
            belongings.Add(puttingObject.id, 1);

        txtBelongings.text = string.Empty;
        foreach (KeyValuePair<TestGrabbableObject.EItemID, int> pair in belongings)
        {
            txtBelongings.text += pair.Key + " " + pair.Value + "\n";
        }
    }
}
