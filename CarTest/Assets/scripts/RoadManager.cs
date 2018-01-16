using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour {
    [SerializeField] public List<RoadConfig> roadMap;
    
    //assigns begin and end values to Road Configurations
    public void Sort()
    {
        //holds current positions in branches
        float[] branches = new float[NumBranches()];
        //loop through every configuration
        foreach(RoadConfig config in roadMap) {
            //make sure Hill length is a multiple of 4
            if(config.roadType == RoadType.Hill)
            {
                config.length = Mathf.Round(config.length / 4) * 4;
            }
            //length of all split sections should be 1
            if(config.roadType == RoadType.Split)
            {
                config.length = 1;
            }
            //assign values and increase position in branch 
            config.Begin = branches[config.branch];
            branches[config.branch] += config.length;
            config.End = branches[config.branch] - 1;
        }
    }
    public int NumBranches() {
        int max = 0;
        foreach(RoadConfig config in roadMap)
        {
            if(config.branch > max) { max = config.branch; }
        }
        return max + 1;
    }
}
