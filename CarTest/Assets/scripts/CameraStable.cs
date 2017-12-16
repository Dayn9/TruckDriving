using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraLocation { TopView, DriverView, RearView }

public class CameraStable : MonoBehaviour {

    [SerializeField] GameObject truck;
    [SerializeField] GameObject cam;
    private float truckY;
    private float scrollY;
    private float scrollX;
    private CameraLocation location = CameraLocation.DriverView;

    // Update is called once per frame
    void Update () {

        truckY = truck.transform.eulerAngles.y;

        //pan camera left and right
        if(Mathf.Abs(Input.GetAxis("Mouse X")) > 0.1f) { scrollY += Input.GetAxis("Mouse X") * 3; }
        //if not panning, return to origional position
        else { scrollY = scrollY / 1.2f; }
        //remove asymptote as scrollY approaches 0
        if (Mathf.Abs(scrollY) < 0.05) { scrollY = 0; }
        //swiches camera to right or left side when it passes in front of camera
        if (Mathf.Abs(scrollY) > 180) { scrollY -= 360 * Mathf.Sign(scrollY); }

        //pan camera up and down
        if (Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.1f) { scrollX += Input.GetAxis("Mouse Y") *1.5f; }

        //Debug.Log(cam.transform.localPosition);
        //Debug.Log(cam.transform.localRotation); 

        //camera views locations and rotations 
        if (Input.GetButtonDown("LeftButton"))
        {
            switch (location) {
                case CameraLocation.TopView:
                    cam.transform.localPosition = new Vector3(0.0f, 5.6f, -6.2f);
                    cam.transform.localRotation = new Quaternion(0.27f, 0.0f, 0.0f, 1.0f);
                    location = CameraLocation.DriverView;
                    break;
                case CameraLocation.DriverView:
                    cam.transform.localPosition = new Vector3(0.0f, -0.5f, 0.75f);
                    cam.transform.localRotation = new Quaternion(0.20f, 0.0f, 0.0f, 1.0f);
                    location = CameraLocation.RearView;
                    break;
                case CameraLocation.RearView:
                    cam.transform.localPosition = new Vector3(0.1f, 1.85f, 5.0f);
                    cam.transform.localRotation = new Quaternion(0.0f, 1.0f, -0.2f, 0.0f);
                    location = CameraLocation.TopView;
                    break;
            }
        }
        //lock camera rotation
        transform.eulerAngles = new Vector3(scrollX, truckY + scrollY,0.0f);
    }
}