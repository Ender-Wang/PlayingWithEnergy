using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBuildingColor : MonoBehaviour
{
    [Tooltip("Get all Buildings")]
    public GameObject buildings;
    [Tooltip("Get all Materials")]
    public Material[] materials; //list to hold the materials in dir /Assets/Materials/Building materials

    // Start is called before the first frame update
    void OnEnable()
    {
        int buildingLength = buildings.transform.childCount;
        for (int i = 0; i < buildingLength; i++)
        {
            GameObject building = buildings.transform.GetChild(i).gameObject;
            if (i < buildingLength / 4)
            {
                building.GetComponent<MeshRenderer>().material = materials[0];
            }
            else if (i >= buildingLength / 4 && i < buildingLength / 2)
            {
                building.GetComponent<MeshRenderer>().material = materials[1];
            }
            else if (i > buildingLength / 2 && i < buildingLength * 3 / 4)
            {
                building.GetComponent<MeshRenderer>().material = materials[2];
            }
            else
            {
                building.GetComponent<MeshRenderer>().material = materials[3];
            }
        }
        // Debug.Log("# of Children building: " + buildings.transform.childCount);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
