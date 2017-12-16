using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerController : MonoBehaviour
{
    [SerializeField] private float antiRoll;
    [SerializeField] private List<WheelSet> wheelSets;
    [SerializeField] private float maxDownForce;

    void FixedUpdate()
    {
        Adjust();
        foreach (WheelSet wheelSet in wheelSets)
        {
            Stabilize(wheelSet);
            SetWheelMeshes(wheelSet);
        }
    }

    //borrows Stabalize and Set Wheel Position from truck controller
    public void Stabilize(WheelSet wheelSet)
    {
        WheelHit hit;
        float travelRight = 1.0f;
        float travelLeft = 1.0f;
        bool groundedRight = wheelSet.rightWheelCollider.GetGroundHit(out hit);
        if (groundedRight)
        {
            travelRight = (-wheelSet.rightWheelCollider.transform.InverseTransformDirection(hit.point).y - wheelSet.rightWheelCollider.radius) / wheelSet.rightWheelCollider.suspensionDistance;
        }
        bool groundedLeft = wheelSet.leftWheelCollider.GetGroundHit(out hit);
        if (groundedLeft)
        {
            travelLeft = (-wheelSet.leftWheelCollider.transform.InverseTransformDirection(hit.point).y - wheelSet.leftWheelCollider.radius) / wheelSet.leftWheelCollider.suspensionDistance;
        }
        float antiRollForce = (travelLeft - travelRight) * antiRoll;
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
}
