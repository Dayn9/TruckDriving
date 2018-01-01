using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TruckController))] 
public class TruckUserController : MonoBehaviour {

    [SerializeField] private bool XBOX;
    TruckController truck;

	// Gets the Truck Controller Script
	void Awake () {
        truck = GetComponent<TruckController>();
	}
	
	// Gets player inputs and calls move method in Truck Controller
	void FixedUpdate () {
        if (XBOX)
        {
            float horizontal = Input.GetAxis("Steer");
            float vertical = Input.GetAxis("RightTrigger") * -1;
            float brake = Input.GetAxis("LeftTrigger");
            bool handbrake = Input.GetButton("Handbrake");
            bool nitro = Input.GetButton("Nitro");

            truck.Move(horizontal, vertical, brake, handbrake, nitro);
        }
	}
}
