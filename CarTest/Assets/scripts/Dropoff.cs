using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropoff : MonoBehaviour {

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "TruckHead")
        {
            Debug.Log("entered");
            //ERROR -------------------------------------------------------------------------------------------------------------<<<
            GameObject trailer = coll.transform.root.gameObject.GetComponent<TruckController>().removeTrailer();

        }
    }
}
