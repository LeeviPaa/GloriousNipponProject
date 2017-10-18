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
            if (state == EBagState.Grabed)
            {
                OnCarry();
                return;
            }
            if (state == EBagState.Carried)
            {
                OnGrab();
                return;
            }

            print("pushQ");
        }

       
        print(grabbingObjects.Count);
        if (state == EBagState.Carried)
        {
           //I want to fix to back of maincam(vr cam).
            transform.SetParent(bagpos);
            transform.position = Camera.main.transform.position;
            transform.localRotation = Quaternion.identity;
        }

        //if (state == EBagState.Grabed)
        //{
        //    transform.SetParent(handpos);
        //    transform.localPosition = Vector3.zero;
        //    transform.localRotation = Quaternion.identity;
        //}

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
    }

    public void OnGrab()
    {
        state = EBagState.Grabed;

    }
    private void OnTriggerEnter(Collider other)
    {
        var obj = other.GetComponent<TestGrabbableObject>();
        if (obj)
        {
                grabbingObjectsTemp.Add(obj);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        //if (other.gameObject.GetComponent<TestGrabbableObject>())
        //{
        //        print("You can loot if you release grabbing object");
        
        //        // When release grabed object in the bag
        //        if (other.gameObject.GetComponent<TestGrabbableObject>().is == false)
        //        {

        //            print("get");
        //            PutTresure(g);
        //        }
        //    }
        //}
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
