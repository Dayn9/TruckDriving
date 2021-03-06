﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckController : MonoBehaviour {

    [SerializeField] private GameObject trailerPrefab;
    //modifiers for forces
    [SerializeField] private float maxMotor;
    [SerializeField] private float maxBrake;
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float maxDownForce;
    [SerializeField] private float maxNitro;
    [SerializeField] private float antiRoll;
    //set of 2 wheels, both colliders and meshes
    [SerializeField] private List<WheelSet> wheelSets;

    private Rigidbody rb;
    private List<GameObject> trailers;
    private float currentSteerAngle;

    //for locking the speed
    private float currentMagnitude;
    //private float setMagnitude = 0.0f;
    private float setTorque;

    public void Start()
    {
        //lower center of mass so truck doesn't tip as easily
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = rb.centerOfMass + new Vector3(0.0f, -1.0f, 0.0f);
        currentSteerAngle = maxSteerAngle;
        currentMagnitude = 0.0f;

        trailers = new List<GameObject>();
    }

    //Triggers
    public virtual void OnTriggerEnter(Collider coll)
    {
        //Add trailers to back of truck
        if (coll.tag == "TrailerPickup")
        {
            coll.gameObject.SetActive(false);
            GameObject newTrailer;
            //first trailer gets added to back of truck head
            if(trailers.Count == 0) 
            {
                newTrailer = Instantiate(trailerPrefab, transform.position, transform.rotation);
                //connect the newTrailer and match velocity
                newTrailer.GetComponent<HingeJoint>().connectedBody = rb;
                newTrailer.GetComponent<Rigidbody>().velocity = rb.velocity;
            }
            //all others added behind the last added trailer
            else
            {
                newTrailer = Instantiate(trailerPrefab, trailers[trailers.Count-1].transform.position + (trailers[trailers.Count - 1].transform.forward * -5.5f), trailers[trailers.Count - 1].transform.rotation);
                newTrailer.GetComponent<HingeJoint>().connectedBody = trailers[trailers.Count - 1].GetComponent<Rigidbody>();
                newTrailer.GetComponent<Rigidbody>().velocity = trailers[trailers.Count - 1].GetComponent<Rigidbody>().velocity;
            }
            //give the trailer the same player number as the truck head
            newTrailer.GetComponent<TruckUserController>().PlayerNum = gameObject.GetComponent<TruckUserController>().PlayerNum;
            trailers.Add(newTrailer);
        }
    } 

    //removes last trailer from back and sends it to dropOff
    public virtual GameObject RemoveTrailer()
    {
        trailers.RemoveAt(trailers.Count-1);
        return trailers[trailers.Count - 1];
    }

    //called when player falls off the map
    public virtual void BackToStart()
    {
        //remove all trailers except for first one
        while (trailers.Count > 1)
        {
            RemoveTrailer();
        }
        //stop
        rb.velocity = Vector3.zero;
        //return to starting position
        transform.rotation = Quaternion.identity;
        trailers[0].transform.rotation = Quaternion.identity;
        transform.position = new Vector3(0,30,0);
        trailers[0].transform.position = Vector3.zero;
    }


    public void Move(float horizontal, float acc, float brk, bool handbrake, bool nitro)
    {
        //clamp range on input values
        float acceleration = Mathf.Clamp(acc, 0, 1f);
        float brake = Mathf.Clamp(brk, -1f, 0);
        float steering = Mathf.Clamp(horizontal, -0.7f, 0.7f);

        currentMagnitude = Adjust();

        foreach (WheelSet wheelSet in wheelSets)
        {
            //sets the angle of any steerWheels
            if (wheelSet.steerWheel)
            {
                steering = steering * currentSteerAngle;
                wheelSet.rightWheelCollider.steerAngle = steering;
                wheelSet.leftWheelCollider.steerAngle = steering;
                //nitro locks the wheels forward
                if (nitro) {
                    wheelSet.rightWheelCollider.steerAngle = 0;
                    wheelSet.leftWheelCollider.steerAngle = 0;
                }
            }
            //applys torques to any driveWheels
            if (wheelSet.driveWheel)
            {
                //apply force evenly across all driveWheels
                float thrustTorque = acceleration * (maxMotor / NumDriveWheels());
                //add a lot of torque when going slowly
                if(currentMagnitude < 10 && acceleration > 0)
                {
                    thrustTorque += maxMotor;
                }
                wheelSet.rightWheelCollider.motorTorque = thrustTorque;
                wheelSet.leftWheelCollider.motorTorque = thrustTorque;
                //override forward torque with brake torques 
                if (brake < 0)
                {
                    float brakeTorque = brake * (maxBrake / NumDriveWheels());
                    wheelSet.rightWheelCollider.motorTorque = brakeTorque;
                    wheelSet.leftWheelCollider.motorTorque = brakeTorque;
                }
                //nitro maintains the speed traveling when first engaged -------------------------------------------<< NOT QUITE RIGHT
                /*if (nitro)
                {
                    if (setMagnitude == 0.0f)
                    {
                        setMagnitude = currentMagnitude;
                        setTorque = thrustTorque/2;
                    }
                    //adjust setTorque to maintain origional speed
                    Debug.Log(setMagnitude + " "+ currentMagnitude);
                    if (currentMagnitude > setMagnitude)
                    {
                        setTorque -= 10.0f;
                    }
                    else if (currentMagnitude < setMagnitude)
                    {
                        setTorque += 10.0f;
                    }
                    wheelSet.rightWheelCollider.motorTorque = setTorque;
                    wheelSet.leftWheelCollider.motorTorque = setTorque;
                }*/
            }
            //apply brake forces when handbrake engaged
            wheelSet.rightWheelCollider.brakeTorque = 0.0f;
            wheelSet.leftWheelCollider.brakeTorque = 0.0f;
            if (handbrake)
            {
                wheelSet.rightWheelCollider.brakeTorque = maxBrake;
                wheelSet.leftWheelCollider.brakeTorque = maxBrake;
            }

            Stabilize(wheelSet);
            // FIX rotates on wrong axis
            SetWheelMeshes(wheelSet);
            if (transform.position.y < -10)
            {
                BackToStart();
            }
        }
    }

    //transfer some compression force from one spring to the opposite on same axle
    public void Stabilize(WheelSet wheelSet) {
        WheelHit hit;
        //travel values between 0.0 (fully compressed) and 1.0 (fully extended)
        //if not on the ground must be fully extended
        float travelRight = 1.0f;
        float travelLeft = 1.0f;
        bool groundedRight = wheelSet.rightWheelCollider.GetGroundHit(out hit);
        if (groundedRight)
        {
            //find the y coordinate of the hit point in world space 
            travelRight = (-wheelSet.rightWheelCollider.transform.InverseTransformDirection(hit.point).y - wheelSet.rightWheelCollider.radius) / wheelSet.rightWheelCollider.suspensionDistance;
        }
        else
        {
            wheelSet.rightWheelCollider.motorTorque = 0;
        }
        bool groundedLeft = wheelSet.leftWheelCollider.GetGroundHit(out hit);
        if (groundedLeft)
        {
            travelLeft = (-wheelSet.leftWheelCollider.transform.InverseTransformDirection(hit.point).y - wheelSet.leftWheelCollider.radius) / wheelSet.leftWheelCollider.suspensionDistance;
        }
        else
        {
            wheelSet.leftWheelCollider.motorTorque = 0;
        }
        //antiRollForce to be transfered depends on difference in suspension 
        float antiRollForce = (travelLeft - travelRight) * antiRoll;
        //subtract the force from one spring and add it to the other
        if (groundedRight)
        {
            wheelSet.rightWheelCollider.attachedRigidbody.AddForceAtPosition(wheelSet.rightWheelCollider.transform.up * (antiRollForce * -1), wheelSet.rightWheelCollider.transform.position);
        }
        if (groundedLeft)
        {
            wheelSet.leftWheelCollider.attachedRigidbody.AddForceAtPosition(wheelSet.leftWheelCollider.transform.up * (antiRollForce * -1), wheelSet.leftWheelCollider.transform.position);
        }
    }

    public float Adjust()
    {
        float magnitude = wheelSets[0].rightWheelCollider.attachedRigidbody.velocity.magnitude;
        //adds a downforce to truck to help it self right and stay down at high speed
        wheelSets[0].rightWheelCollider.attachedRigidbody.AddForce(-transform.up * maxDownForce * magnitude);
        //increase the down force
        wheelSets[0].rightWheelCollider.attachedRigidbody.AddForce(Vector3.down * maxDownForce * magnitude);
        return magnitude;
    }

    //alligns the Wheel meshes with the position and rotation of the wheel colliders
    public void SetWheelMeshes(WheelSet wheelSet)
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

    //returns the number of drive wheels
    public int NumDriveWheels()
    {
        int wheels = 0;
        foreach(WheelSet wheelSet in wheelSets)
        {
            if (wheelSet.driveWheel)
            {
                wheels += 2;
            }
        }
        return wheels;
    }
}
