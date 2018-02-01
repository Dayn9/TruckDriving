using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishLine : MonoBehaviour {

    [SerializeField] private Text lapCounter;
	
    private int Lap = 0;

	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("TruckHead"))
        {
            Lap++;
            lapCounter.text = "Lap: " + Lap;
        }   
    }
}
