using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WheelSet : System.Object {
    [SerializeField] public WheelCollider rightWheelCollider;
    [SerializeField] public GameObject rightWheel;
    [SerializeField] public WheelCollider leftWheelCollider;
    [SerializeField] public GameObject leftWheel;
    [SerializeField] public bool driveWheel;
    [SerializeField] public bool steerWheel;
}
