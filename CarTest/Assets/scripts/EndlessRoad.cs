using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessRoad : MonoBehaviour {

    [SerializeField] private GameObject mainCamera;
    //dimensions of road gameobject
    private float width;
    private float length;
    private float height;
    //angles in degress the road turns or ramps
    [SerializeField] private float curveAngle;
    [SerializeField] private float rampAngle;
    [SerializeField] private GameObject controller;
    [SerializeField] private int branch;

    public static List<RoadConfig> roadMap;
    //index of branches is branch, value is count of that branch
    public static List<int> branches = new List<int>();

    private bool created = false;
    private static RoadManager manager;
    private float maxBranch;

    [SerializeField] private GameObject pickupPrefab;


    private void Start()
    {
        width = transform.localScale.x;
        length = transform.localScale.z;
        height = transform.localScale.y;

        if (roadMap == null) {
            manager = controller.GetComponent<RoadManager>();
            manager.Sort();
            roadMap = manager.roadMap;

            //start initial branch
            branches.Add(0);
            branch = 0;
        }

        maxBranch = manager.branches[branch] - 1;

        //every 10 road segments, create a trailer pickup with a random horizontal position 
        if (branches[branch] % 10 == 0 && branches[branch] != 0)
        {
            Instantiate(pickupPrefab, transform.position + new Vector3(Random.Range((-width/2) + 2, (width/2) - 2), 0, 0), transform.rotation);
        }

    }

    void Update () {
        //only make a new road section if haven't already 
        if (!created && branches[branch] < maxBranch) {
            //check if within distance of camera render
            if (Mathf.Sqrt(Mathf.Pow(transform.position.x - mainCamera.transform.position.x, 2) 
                + Mathf.Pow(transform.position.y - mainCamera.transform.position.y, 2) 
                + Mathf.Pow(transform.position.z - mainCamera.transform.position.z, 2)) 
                < mainCamera.GetComponent<Camera>().farClipPlane)
            {
                GameObject newRoad;
                foreach (RoadConfig config in roadMap)
                {
                    //check if beggining of new road configuration
                    if (branch == config.branch && branches[branch] == config.Begin )
                    {
                        //if so set the new curve and ramp angle based on the configuration of the road
                        switch (config.roadType)
                        {
                            case RoadType.Straight:
                                curveAngle = 0;
                                rampAngle = 0;
                                break;
                            case RoadType.Curved:
                                curveAngle = config.curveAngle;
                                rampAngle = 0;
                                break;
                            case RoadType.Ramp:
                                curveAngle = 0;
                                rampAngle = config.rampAngle;
                                break;
                            case RoadType.Split:
                                //contiue current path with same ramp or curve
                                newRoad = Instantiate(gameObject);
                                Position(newRoad);
                                branches[branch]++;
                                //create new branch
                                curveAngle = config.curveAngle;
                                branches.Add(0);
                                branch = branches.Count - 1;
                                break;
                        }
                    }
                }
                //create a copy of self with same position and rotation
                newRoad = Instantiate(gameObject);
                Position(newRoad);
                branches[branch]++;

                created = true;
            }
        }
	}

    //length of each hill and magnitude of angle
    private void Hill(float length, float magnitude)
    {
        rampAngle = branches[branch] % length < length / 4 || branches[branch] % length >= 3 * length / 4 ? -magnitude : magnitude;
    }

    //move and rotate to new position
    private void Position(GameObject newRoad)
    {
        //move center of new Road to front of origional
        newRoad.transform.Translate(Vector3.forward * (length / 2));
        //Ramp up and down
        newRoad.transform.Translate(Vector3.up * (height / 2) * Mathf.Sign(rampAngle));
        newRoad.transform.Rotate(Vector3.right, rampAngle);
        newRoad.transform.Translate(Vector3.down * (height / 2) * Mathf.Sign(rampAngle));
        //Turn left and right
        newRoad.transform.Translate(Vector3.left * (width / 2) * Mathf.Sign(curveAngle));
        newRoad.transform.Rotate(Vector3.up, curveAngle);
        newRoad.transform.Translate(Vector3.right * (width / 2) * Mathf.Sign(curveAngle));
        //move into final position
        newRoad.transform.Translate(Vector3.forward * (length / 2));

        //rename based on position
        newRoad.name = "Road_" + (int)transform.position.x + "_" + (int)transform.position.z;
    }
}