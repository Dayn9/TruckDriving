using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStable : MonoBehaviour {

    [SerializeField] GameObject truck;
    private float truckY;
    private float scrollY;
    private float scrollX;

	// Update is called once per frame
	void Update () {
        truckY = truck.transform.eulerAngles.y;
        if(Mathf.Abs(Input.GetAxis("Mouse X")) > 0.1f) { scrollY -= Input.GetAxis("Mouse X") * 3; }
        if (Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.1f) { scrollX += Input.GetAxis("Mouse Y") * 3; }

        //lock camera rotation
        transform.eulerAngles = new Vector3(scrollX, truckY + scrollY,0.0f);

    }
}
