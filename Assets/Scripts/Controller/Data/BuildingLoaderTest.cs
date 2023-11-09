using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLoaderTest : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject building;
    void Start()
    {
        // building = SelectingController.selectedBuilding;
    }

    // Update is called once per frame
    void Update()
    {
        if (building != SelectingController.selectedBuilding)
        {
            building = SelectingController.selectedBuilding;
            Debug.Log("Building selected");
            if (building != null)
            {
                var dic = BuildingLoader.Instance.getBuildingInfo(building);
                var keys = dic.Keys;
                foreach (var key in keys)
                {
                    Debug.Log(key + " " + dic[key]);

                }

            }

        }
        // var names = BuildingLoader.Instance.getSemanticNames();
        // foreach (var name in names)
        // {
        //     Debug.Log(name);
        // }
        // var dic = BuildingLoader.Instance.getSemanticData("Potenzial fur Grundwasserwarmepumpen auf Blockebene");
        // foreach (var key in dic.Keys)
        // {
        //     Debug.Log(key + " " + dic[key]);
        // }

    }
}
