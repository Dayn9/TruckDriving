﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckController : MonoBehaviour {

    //modifiers for forces
    [SerializeField] private float maxMotor;
    [SerializeField] private float maxBrake;
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float maxHandbrake;
    [SerializeField] private float maxDownForce;

    //set of 2 wheels, both colliders and meshes
    [SerializeField] private List<WheelSet> wheelSets;

    public void Move(float horizontal, float vertical, float jump)
    {
        float acceleration = Mathf.Clamp(vertical, 0, 1);
        float brake = Mathf.Clamp(vertical, -1, 0);
        float steering = Mathf.Clamp(horizontal, -1, 1);
        float handbrake = Mathf.Clamp(jump, 0, 1);


        //as velocity increases: decrease brake forces and decrease maxSteerAngle and add down force
        Adjust();

        foreach (WheelSet wheelSet in wheelSets)
        {

            if (wheelSet.steerWheel)
            {
                float steerAngle = steering * maxSteerAngle;
                wheelSet.rightWheelCollider.steerAngle = steerAngle;
                wheelSet.leftWheelCollider.steerAngle = steerAngle;
            }
            if (wheelSet.driveWheel)
            {
                // FIX should divide by number of driveWheels
                float thrustTorque = acceleration * (maxMotor / (wheelSets.Count * 2));
                wheelSet.rightWheelCollider.motorTorque = thrustTorque;
                wheelSet.leftWheelCollider.motorTorque = thrustTorque;
                if (brake < 0)
                {
                    float brakeTorque = brake * (maxBrake / (wheelSets.Count * 2));
                    wheelSet.rightWheelCollider.motorTorque = brakeTorque;
                    wheelSet.leftWheelCollider.motorTorque = brakeTorque;
                }
            }

            if (handbrake > 0)
            {
                float handbrakeTorque = handbrake * maxHandbrake;
                wheelSet.rightWheelCollider.brakeTorque = handbrakeTorque;
                wheelSet.leftWheelCollider.brakeTorque = handbrakeTorque;
                wheelSet.rightWheelCollider.motorTorque = 0;
                wheelSet.leftWheelCollider.motorTorque = 0;
            }
            else
            {
                wheelSet.rightWheelCollider.brakeTorque = 0;
                wheelSet.leftWheelCollider.brakeTorque = 0;

            }
            // FIX rotates on wrong axis
            //setWheelMeshes(wheelSet);
        }
    }

    public void Adjust()
    {
        float magnitude = wheelSets[0].rightWheelCollider.attachedRigidbody.velocity.magnitude;
        wheelSets[0].rightWheelCollider.attachedRigidbody.AddForce(-Vector3.up * maxDownForce * magnitude);
    }

    //alligns the Wheel meshes with the position and rotation of the wheel colliders
    public void setWheelMeshes(WheelSet wheelSet)
    {
        Quaternion rotation;
        Vector3 position;
        wheelSet.rightWheelCollider.GetWorldPose(out position, out rotation);
        wheelSet.rightWheel.transform.position = position;
        wheelSet.rightWheel.transform.rotation = rotation;
        wheelSet.leftWheelCollider.GetWorldPose(out position, out rotation);
        wheelSet.leftWheel.transform.position = position;
        wheelSet.leftWheel.transform.rotation = rotation;
    }

}
