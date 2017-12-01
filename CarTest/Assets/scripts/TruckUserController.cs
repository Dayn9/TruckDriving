using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TruckController))] 
public class TruckUserController : MonoBehaviour {

    TruckController truck;

	// Gets the Truck Controller Script
	void Awake () {
        truck = GetComponent<TruckController>();
	}
	
	// Gets player inputs and calls move method in Truck Controller
	void FixedUpdate () {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float jump = Input.GetAxis("Jump");

        truck.Move(horizontal, vertical, jump);
	}
}
