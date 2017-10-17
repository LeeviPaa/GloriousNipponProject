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
    private Rigidbody rb;
    [SerializeField]
    private BoxCollider col;
    [SerializeField]
    private AudioSource sound;
    [SerializeField]
    private UnityEngine.UI.Text txtBelongings;

    List<TestGrabbableObject> grabbingObjectsTemp;

    GameObject followObject = Camera.main.gameObject;

    private Dictionary<TestGrabbableObject.EItemID, int> belongings = new Dictionary<TestGrabbableObject.EItemID, int>();

    TestGrabbableObject obj;

    // Use this for initialization
    void Start()
    {
        grabbingObjectsTemp = new List<TestGrabbableObject>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();


        // For camera
        //transform.position = Camera.main.transform.position - Camera.main.transform.forward;
        //transform.forward = Camera.main.transform.forward;

        // For hands.
        if (followObject.GetComponent<Camera>())
        {
            transform.position = Camera.main.transform.position - Camera.main.transform.forward;
            transform.localRotation = Camera.main.transform.localRotation;
            transform.forward = Camera.main.transform.forward;
        }
        else
        {
            transform.position = followObject.transform.position;
            transform.localRotation = followObject.transform.localRotation;
        }


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
        if (temp.GetIsGrabbed())
        {
            obj = temp;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (obj)
        {
            if (!obj.GetIsGrabbed())
            {
                PutTresure(obj);
            }
        }
    }


    void PutTresure(TestGrabbableObject grabbingObject)
    {
        IncreaseItemInBag(grabbingObject.id);


        sound.PlayOneShot(sound.clip);
        // sound.PlayOneShot(grabbingObject.clip);

        print("PuttingToBag!!");


        //In future, chenge to animation .
        Destroy(grabbingObject.gameObject);

    }

    void IncreaseItemInBag(TestGrabbableObject.EItemID id)
    {
        if (belongings.ContainsKey(id))
            belongings[id]++;
        else
            belongings.Add(id, 1);

        txtBelongings.text = string.Empty;
        foreach (KeyValuePair<TestGrabbableObject.EItemID, int> pair in belongings)
        {
            txtBelongings.text += pair.Key + " " + pair.Value + "\n";
        }
    }
}
