using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessRoad : MonoBehaviour {

    [SerializeField] private GameObject mainCamera;
    [SerializeField] private float width;
    [SerializeField] private float length;
    [SerializeField] private float curveAngle;
    [SerializeField] private GameObject road;

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
                //create a copy of self with same position and rotation
                GameObject newRoad = Instantiate(gameObject);
                //move to anchor point, rotate and then move to final point
                if (curveAngle > 0)
                {
                    //anchor point in upper left of origional road
                    newRoad.transform.Translate(Vector3.forward * (length / 2) + Vector3.left * (width / 2));
                    newRoad.transform.Rotate(Vector3.up, curveAngle);
                    newRoad.transform.Translate(Vector3.forward * (length / 2) + Vector3.right * (width / 2));
                }
                else if (curveAngle < 0)
                {
                    //anchor point in upper right of origional road
                    newRoad.transform.Translate(Vector3.forward * (length / 2) + Vector3.right * (width / 2));
                    newRoad.transform.Rotate(Vector3.up, curveAngle);
                    newRoad.transform.Translate(Vector3.forward * (length / 2) + Vector3.left * (width / 2));
                }
                else {
                    //just move road to directly in front
                    newRoad.transform.Translate(Vector3.forward * length);
                }
                created = true;
            }
        }
	}
}
/* ALOMOST WORKED
 * //create a new Road and rotate it curveAngle degrees
                GameObject newRoad = new GameObject();
                newRoad.transform.rotation = transform.rotation;
                newRoad.transform.position = transform.position;
                newRoad.transform.Rotate(Vector3.up, curveAngle);

                if (curveAngle > 0) {
                    float rotation = transform.rotation.y * 2;
                    Vector3 orginTopLeft = new Vector3(transform.position.x - ((width / 2) * Mathf.Cos(rotation)) + ((length / 2) * Mathf.Sin(rotation)), transform.position.y, transform.position.z + ((width / 2) * Mathf.Sin(rotation)) + ((length / 2) * Mathf.Cos(rotation)));
                    Debug.Log(orginTopLeft);
                    rotation = newRoad.transform.rotation.y * 2;
                    Vector3 newBottomLeft = new Vector3(newRoad.transform.position.x - ((width / 2) * Mathf.Cos(rotation)) - ((length / 2) * Mathf.Sin(rotation)), transform.position.y, newRoad.transform.position.z + ((width / 2) * Mathf.Sin(rotation)) - ((length / 2) * Mathf.Cos(rotation)));
                    Debug.Log(newBottomLeft);
                    newRoad.transform.position = transform.position + orginTopLeft - newBottomLeft;
                }
*/