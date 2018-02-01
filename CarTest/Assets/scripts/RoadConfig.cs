using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoadType { Straight, Curved, Ramp, Hill, Split, End}

[System.Serializable]
public class RoadConfig : System.Object {
    [SerializeField] public RoadType roadType;
    [SerializeField] public int branch;
    [SerializeField] public float curveAngle;
    [SerializeField] public float rampAngle;
    [SerializeField] public float length;

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
