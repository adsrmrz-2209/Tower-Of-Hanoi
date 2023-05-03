//create by Adrian S. Ramirez

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscScript : MonoBehaviour
{
    public Rigidbody rb; // needs to be public for the DiscManager.cs to have access
    BoxCollider boxClldr;

    public int discNum;
    public int discNumSizeBelow;
    public float discYpos; //set in the editor

    [SerializeField] float rayLengthAbove = 0.18f;
    private float defRayLengthAbove;
    [SerializeField] float rayLengthBelow;
    [SerializeField] LayerMask discLayer;

    public bool discAboveDeteceted = false;
    public bool discBelowDeteceted = false;

    public bool isOntrigger;

    RaycastHit hit;
    DiscManager addDisc;

    public Vector3 recordPos;

    void Start()
    {
        defRayLengthAbove = rayLengthAbove;
        rb = GetComponent<Rigidbody>();
        boxClldr = GetComponent<BoxCollider>();
        addDisc = FindObjectOfType<DiscManager>();
    }

    void Update()
    {
        //check if disc detection is working (for debugging purposes only)
        discAboveDeteceted = DetectAboveDisc();
        discBelowDeteceted = DetectBelowDisc();
    }

    public bool DetectAboveDisc()
    {
        //cast raycast above to check if there is a disc above this disc
        Physics.Raycast(transform.position, Vector3.up, out hit, rayLengthAbove, discLayer);

        //return true if raycast is hitting any disc above. false if no disc is hit
        return hit.collider != null ? true : false;
    }

    public bool DetectBelowDisc()
    {
        //cast raycast below check if there is a disc below this disc
        Physics.Raycast(transform.position, -Vector3.up, out hit, rayLengthBelow, discLayer);

        //record the disc value below to determine if the current disc can be inserted
        discNumSizeBelow = hit.collider != null ? hit.collider.GetComponent<DiscScript>().discNum : 0;

        //return true if raycast is hitting any disc below that has lower value. false if no disc is hit
        return discNum > discNumSizeBelow ? true : false;
        
    }


    private void OnMouseDown()
    {
        //record the last position so it can reset back to last position if it didnt touch any stick
        if (!DetectAboveDisc())
        {
            Vector3 recordedPosition = transform.position;
            recordPos = recordedPosition;
        }
    }

    private void OnMouseDrag()
    {
        if (!DetectAboveDisc())
        {
            rayLengthAbove = 0;
            gameObject.layer = 7;

            //IgnoreCollision is added to prevent messing up the conditionsMet variable in DiscManager.cs
            Physics.IgnoreCollision(boxClldr, GameObject.FindGameObjectWithTag("3rdStick").GetComponent<MeshCollider>(), true);

            float dragPosZ = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragPosZ));
            rb.isKinematic = true;
        }
    }

    private void OnMouseUp()
    {
            UIScript ui = FindObjectOfType<UIScript>();
            rayLengthAbove = defRayLengthAbove;
            gameObject.layer = 6;

            //IgnoreCollision is added to prevent messing up the conditionsMet variable in DiscManager.cs
            if (DetectBelowDisc()) Physics.IgnoreCollision(boxClldr, GameObject.FindGameObjectWithTag("3rdStick").GetComponent<MeshCollider>(), false);
            if (DetectBelowDisc() && isOntrigger) ui.moveCount++; 
            if (!isOntrigger || !DetectBelowDisc() || DetectAboveDisc()) transform.position = new Vector3(recordPos.x, recordPos.y, recordPos.z); //reset position if it didn't hit any stick
    }

    private void OnTriggerEnter(Collider other)
    {
        //add 1 to the discManager int variable, this will determine how many disc is on the 3rd stick and decide if the player won or not
        if (other.CompareTag("3rdStick") && DetectBelowDisc()) addDisc.conditionsMet++;
    }

    private void OnTriggerStay(Collider other)
    {
        isOntrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        //minus 1 to the discManager int variable, this will determine how many disc is on the 3rd stick and decide if the player won or not
        if (other.CompareTag("3rdStick") && DetectBelowDisc()) addDisc.conditionsMet--;
        isOntrigger = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject.Find("TapSound").GetComponent<AudioSource>().Play(); //playing the sound from one source for performance friendly
    }
}
