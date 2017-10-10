using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bag : MonoBehaviour
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

    TestGrabbableObject gObj;
    bool readyForIn;
    [SerializeField]
    private float intaractivableRange;


    private Dictionary<TestGrabbableObject.EItemID, int> belongings = new Dictionary<TestGrabbableObject.EItemID, int>();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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
    private void OnCollisionEnter(Collision collision)
    {
        gObj = collision.gameObject.GetComponent<TestGrabbableObject>();
        if (gObj)
            print("hit");
    }
    private void OnTriggerEnter(Collider collider)
    {
        gObj = collider.gameObject.GetComponent<TestGrabbableObject>();
        if (gObj)
            print("hit");
    }

    /// <summary>
    /// Don't need this OnCollisionStay().
    /// Because When from true to false of the isGrabing flag.
    /// Why i don't remove it, for testing program.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay(Collision collision)
    {
        //print("hittttt");
        //if (gObj)
        //{
        //    if (gObj.isGrabing == true)
        //    {
        //        readyForIn = true;
        //    }
        //    // When release grabed object in the bag
        //    if (readyForIn == true && gObj.isGrabing == false)
        //        Destroy(gObj.gameObject);
        //}
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        print("hittttt");
        if (gObj)
        {
            if (gObj.isGrabing == true)
            {
                readyForIn = true;
            }
            // When release grabed object in the bag
            if (readyForIn == true && gObj.isGrabing == false)
                PutTresure();
        }
    }

    void PutTresure()
    {
        IncreaseItemInBag(gObj.id);
        sound.PlayOneShot(sound.clip);
        print("fa");

        Destroy(gObj.gameObject);
        
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
