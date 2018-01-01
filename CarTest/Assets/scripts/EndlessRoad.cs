using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessRoad : MonoBehaviour {

    [SerializeField] private GameObject mainCamera;
    [SerializeField] private float width;

    private bool created = false;

	void Update () {
        //only make a new road section if haven't already
        if (!created) {
            //check if within distance of camera render
            if (Mathf.Sqrt(Mathf.Pow(transform.position.x - mainCamera.transform.position.x, 2) 
                + Mathf.Pow(transform.position.y - mainCamera.transform.position.y, 2) 
                + Mathf.Pow(transform.position.z - mainCamera.transform.position.z, 2)) 
                < mainCamera.GetComponent<Camera>().farClipPlane)
            {
                //create a copy of self 
                Instantiate(gameObject, transform.position + (transform.forward * width), transform.rotation);
                created = true;
            }
        }
	}
}
