using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TruckController))] 
public class TruckUserController : MonoBehaviour {

    TruckController truck;
    [SerializeField] public int PlayerNum;

	// Gets the Truck Controller Script
	void Awake () {
        truck = GetComponent<TruckController>();
	}
	
	// Gets player inputs and calls move method in Truck Controller
	void FixedUpdate () {
        float horizontal = Input.GetAxis("Steer" + PlayerNum);
        float vertical = Input.GetAxis("RightTrigger" + PlayerNum) * -1;
        float brake = Input.GetAxis("LeftTrigger" + PlayerNum);
        bool handbrake = Input.GetButton("Handbrake" + PlayerNum);
        bool nitro = Input.GetButton("Nitro" + PlayerNum);
        truck.Move(horizontal, vertical, brake, handbrake, nitro);
	}
}
