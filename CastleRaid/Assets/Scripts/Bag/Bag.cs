using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bag : VRTK.VRTK_InteractableObject
{

    enum EBagState
    {
        Placed,
        Grabed,
        Carried
    }
    EBagState state = EBagState.Placed;

    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private BoxCollider col;
    [SerializeField]
    private Transform bagpos;
    [SerializeField]
    private Transform handpos;
    [SerializeField]
    private AudioSource sound;
    [SerializeField]
    private UnityEngine.UI.Text txtBelongings;

    List<TestGrabbableObject> grabbingObjectsTemp;
    bool readyForIn;
    [SerializeField]
    private float intaractivableRange;


    private Dictionary<TestGrabbableObject.EItemID, int> belongings = new Dictionary<TestGrabbableObject.EItemID, int>();

    // Use this for initialization
    void Start()
    {
        grabbingObjectsTemp = new List<TestGrabbableObject>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // For non vr.
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (state == EBagState.Placed)
            {
                if (CheckDistance() < intaractivableRange)
                    OnGrab();

                return;
            }

            if (state == EBagState.Grabed)
            {
                OnCarry();
                return;
            }
            if (state == EBagState.Carried)
            {
                OnPlace();
                return;
            }

            print("pushQ");
        }


        if (state == EBagState.Carried)
        {
            transform.SetParent(bagpos);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        if (state == EBagState.Placed)
        {
            transform.SetParent(null);
        }
        if (state == EBagState.Grabed)
        {
            transform.SetParent(handpos);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

    }


    float CheckDistance()
    {
        float result = Vector3.Distance(GameObject.FindWithTag("Player").transform.position, transform.position);
        print(result);
        return result;
    }
    public void OnCarry()
    {
        state = EBagState.Carried;
        col.isTrigger = true;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
    public void OnPlace()
    {
        state = EBagState.Placed;
        col.isTrigger = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
    }
    public void OnGrab()
    {
        state = EBagState.Grabed;
        col.isTrigger = true;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        readyForIn = false;

    }
    private void OnTriggerEnter(Collider other)
    {
        //if (other.GetComponent<TestGrabbableObject>())
        //{
        //    print("touch");
        //    foreach (var g in grabbingObjects)
        //    {
        //        grabbingObjectsTemp.Add(g.GetComponent<TestGrabbableObject>());
        //    }
        //}
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        //if (other.GetComponent<TestGrabbableObject>())
        //    foreach (var g in grabbingObjectsTemp)
        //    {
        //        // When release grabed object in the bag
        //        if (g.IsGrabbed() == false)
        //        {
        //            print("get");
        //            PutTresure(g);
        //        }
        //    }
    }


    void PutTresure(TestGrabbableObject grabbingObject)
    {
        IncreaseItemInBag(grabbingObject.id);

        sound.PlayOneShot(sound.clip);

        print("PuttingToBag!!");

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
