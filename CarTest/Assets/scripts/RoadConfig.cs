﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoadType { Straight, Curved, Ramp, Hill, Split, End, Pickup
}

[System.Serializable]
public class RoadConfig : System.Object {
    [SerializeField] public RoadType roadType;
    [SerializeField] public int branch;
    [SerializeField] public float curveAngle;
    [SerializeField] public float rampAngle;
    [SerializeField] public float length;

    //when true, give the road a powerup section
    private bool powerUp;
    public bool PowerUp { get { return powerUp; } }


    //constructor used for creating road sections via script
    public RoadConfig(RoadType roadType, int branch, float curveAngle, float rampAngle, float length)
    {
        this.roadType = roadType;
        this.branch = branch;
        this.curveAngle = curveAngle;
        this.rampAngle = rampAngle;
        this.length = length;
        powerUp = false;
    }

    //alternative constructor for indicating config has a powerup section
    public RoadConfig(RoadType roadType, int branch, float curveAngle, float rampAngle, float length, bool powerUp)
        : this(roadType, branch, curveAngle, rampAngle, length) {
        this.powerUp = powerUp;
    }
    
    //to be determined by RoadManager
    private float begin;
    //properties for begin and end
    public float Begin
    {
        get { return begin; }
        set { begin = value; }
    }
    //unnessesary ???
    private float end;
    public float End
    {
        get { return end; }
        set { end = value; }
    }
}
