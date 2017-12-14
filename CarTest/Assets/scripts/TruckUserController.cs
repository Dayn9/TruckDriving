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
            bool jump = Input.GetButton("Handbrake");
            bool nitro = Input.GetButton("Nitro");
            truck.Move(horizontal, vertical, brake, jump, nitro);
        }
        else
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            bool jump = Input.GetButton("Jump");
            bool nitro = Input.GetKey(KeyCode.X);

            truck.Move(horizontal, vertical, vertical, jump, nitro);
        }
	}
}
