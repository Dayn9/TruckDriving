using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishLine : MonoBehaviour {

    public Text lapCounter;
	
    private int Lap = 0;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("TruckHead"))
        {
            Lap++;
            lapCounter.text = "";
        }  
    }
}
