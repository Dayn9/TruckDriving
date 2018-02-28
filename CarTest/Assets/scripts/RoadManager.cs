using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour {
    [SerializeField] public List<RoadConfig> roadMap;

    [HideInInspector] public float[] branches; 

    private List<RoadConfig> pickups ;
    
    //assigns begin and end values to Road Configurations
    public void Sort()
    {
        branches = new float[NumBranches()];
        
        //add pickup locations to Road Map
        CreatePickups();

        branches = new float[NumBranches()];
        //loop through every configuration and assign values to special cases
        foreach (RoadConfig config in roadMap) {
            
            switch(config.roadType){
                case RoadType.Hill:
                    //make sure Hill length is a multiple of 4
                    config.length = Mathf.Round(config.length / 4) * 4;
                    break;
                case RoadType.Split:
                    //length of all split sections should be 1
                    config.length = 1;
                    break;
            }
            //assign values and increase position in branch 
            config.Begin = branches[config.branch];
            branches[config.branch] += config.length;
            config.End = branches[config.branch] - 1;
        }
    }

    //adds a series of road configurations to the road map that create a pickup location
    public void CreatePickups()
    {
        pickups = new List<RoadConfig>();
        //finds all the pickup placeholder roads and addsthem to seperate list
        foreach (RoadConfig config in roadMap)
        {
            if (config.roadType == RoadType.Pickup)
            {
                pickups.Add(config);                        
            }
        }
        //adds in the road sections to the roadmap
        foreach (RoadConfig config in pickups)
        {
            // slplit off road and make sure path is straight until they meet back up... 
            int index = roadMap.IndexOf(config);
            roadMap.Insert(index++, new RoadConfig(RoadType.Straight, config.branch, 0, 0, 1)); //resets the straight direction
            roadMap.Insert(index++, new RoadConfig(RoadType.Split, config.branch, config.curveAngle, 0, 1));
            roadMap.Insert(index++, new RoadConfig(RoadType.Straight, config.branch, 0, 0, 15));
            //...and then creates the road that splits off and comes back
            int newBranch = NumBranches();
            roadMap.Insert(index++, new RoadConfig(RoadType.Curved, newBranch, config.curveAngle, 0, 2));
            roadMap.Insert(index++, new RoadConfig(RoadType.Straight, newBranch, 0, 0, 3));
            roadMap.Insert(index++, new RoadConfig(RoadType.Curved, newBranch, -1 * config.curveAngle, 0, 2));
            roadMap.Insert(index++, new RoadConfig(RoadType.Straight, newBranch, 0, 0, 5)); 
            roadMap.Insert(index++, new RoadConfig(RoadType.Curved, newBranch, -1 * config.curveAngle, 0, 2, true)); //Power up right after straight section
            roadMap.Insert(index++, new RoadConfig(RoadType.Straight, newBranch, 0, 0, 3));
            roadMap.Insert(index++, new RoadConfig(RoadType.Curved, newBranch, config.curveAngle, 0, 2));
            //remove the pickup location placeholder
            roadMap.Remove(config);   
        }
    }

    //finds the number of branches in the Road Map
    public int NumBranches() {
        int max = 0;
        foreach(RoadConfig config in roadMap)
        {
            if(config.branch > max) { max = config.branch; }
        }
        return max + 1;
    }
}
