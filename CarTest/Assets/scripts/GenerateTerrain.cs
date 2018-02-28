using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Temporary script from Holistic3D: https://www.youtube.com/watch?v=dycHQFEz8VI

public class GenerateTerrain : MonoBehaviour {

    [SerializeField] private float heightScale;
    [SerializeField] private float detailScale;

	// Use this for initialization
	void Start () {
        //get a list of verticies from the plane mesh
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        Vector3[] verticies = mesh.vertices;
        //loop through every vertex and raise it by a perlin noise function based on world position
        for (int v = 0; v < verticies.Length; v++)
        {
            Debug.Log(Mathf.PerlinNoise((verticies[v].x * 10 + this.transform.position.x) / detailScale,
                                               (verticies[v].z * 10 + this.transform.position.z) / detailScale));
            verticies[v].y = Mathf.PerlinNoise((verticies[v].x*10 + this.transform.position.x) / detailScale,
                                               (verticies[v].z*10 + this.transform.position.z) / detailScale) * heightScale;
        }
        mesh.vertices = verticies;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        this.gameObject.AddComponent<MeshCollider>();
	}
}
