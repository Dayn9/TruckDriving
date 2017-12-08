using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStable : MonoBehaviour {

    [SerializeField] GameObject truck;
    private float truckY;

	// Update is called once per frame
	void Update () {
        truckY = truck.transform.eulerAngles.y;
        //lock camera rotation
        transform.eulerAngles = new Vector3(0.0f,truckY,0.0f);


    }
}
