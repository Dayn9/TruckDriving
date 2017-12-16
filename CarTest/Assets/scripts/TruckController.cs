using System.Collections;
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

    public void Start()
    {
        //lower center of mass so truck doesn't tip as easily
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = rb.centerOfMass + new Vector3(0.0f, -1f, 0.0f);
        trailers = new List<GameObject>();
    }

    //Triggers
    void OnTriggerEnter(Collider coll)
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
            trailers.Add(newTrailer);
        }
    }

    //removes last trailer from back and sends it to dropOff
    public GameObject removeTrailer()
    {
        trailers.RemoveAt(trailers.Count-1);
        return trailers[trailers.Count - 1];
    }


    public void Move(float horizontal, float acc, float brk, bool handbrake, bool nitro)
    {
        //clamp range on input values
        float acceleration = Mathf.Clamp(acc, 0, 1f);
        float brake = Mathf.Clamp(brk, -1f, 0);
        float steering = Mathf.Clamp(horizontal, -0.7f, 0.7f);

        //as velocity increases: decrease brake forces and decrease maxSteerAngle and add down force
        Adjust();

        foreach (WheelSet wheelSet in wheelSets)
        {

            if (wheelSet.steerWheel)
            {
                steering = steering * maxSteerAngle;
                wheelSet.rightWheelCollider.steerAngle = steering;
                wheelSet.leftWheelCollider.steerAngle = steering;
            }
            if (wheelSet.driveWheel)
            {
                float thrustTorque = acceleration * (maxMotor / NumDriveWheels());
                if (rb.velocity.magnitude < 0.1)
                {
                    thrustTorque = maxMotor / NumDriveWheels();
                }
                wheelSet.rightWheelCollider.motorTorque = thrustTorque;
                wheelSet.leftWheelCollider.motorTorque = thrustTorque;
                if (brake < 0)
                {
                    float brakeTorque = brake * (maxBrake / NumDriveWheels());
                    wheelSet.rightWheelCollider.motorTorque = brakeTorque;
                    wheelSet.leftWheelCollider.motorTorque = brakeTorque;
                }
            }
            if (nitro)
            {
                rb.AddForce(transform.forward * maxNitro, ForceMode.Impulse);
            }
            if (handbrake)
            {
                wheelSet.rightWheelCollider.motorTorque = 0;
                wheelSet.leftWheelCollider.motorTorque = 0;
            }
            else
            {
                wheelSet.rightWheelCollider.brakeTorque = 0;
                wheelSet.leftWheelCollider.brakeTorque = 0;
            }

            Stabilize(wheelSet);
            // FIX rotates on wrong axis
            SetWheelMeshes(wheelSet);
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
    public void Adjust()
    {
        float magnitude = wheelSets[0].rightWheelCollider.attachedRigidbody.velocity.magnitude;
        //adds a downforce to truck to help it self right and stay down at high speed
        wheelSets[0].rightWheelCollider.attachedRigidbody.AddForce(-transform.up * maxDownForce * magnitude);
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
