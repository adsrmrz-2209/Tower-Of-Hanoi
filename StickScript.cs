using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        DiscScript disc = other.GetComponent<DiscScript>();
        //check if the disc below is bigger than the current disc at mouse drag to see if it is insertable
        if (other.CompareTag("disc"))
        {
            if (disc.DetectBelowDisc())
            {
                disc.GetComponent<Rigidbody>().isKinematic = false;
                disc.transform.position = new Vector3(this.gameObject.transform.position.x, disc.transform.position.y, this.gameObject.transform.position.z);  
            }
        }
    }
}
