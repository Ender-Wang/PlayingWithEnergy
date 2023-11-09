using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingColoringWithData : MonoBehaviour
{
    public static BuildingColoringWithData Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private GameObject[] buildings;    //building prefabs
    private List<GameObject> buildingObjects; //building objects in the scene
    private List<Color> originColor; // original color of the buildings

    void Start()
    {
        buildings = GameObject.FindGameObjectsWithTag("Selectable");
        buildingObjects = new List<GameObject>();
        originColor = new List<Color>();
        for (int i = 0; i < buildings.Length; i++)
        {
            for (int j = 0; j < buildings[i].transform.childCount; j++)
            {
                var child = buildings[i].transform.GetChild(j).gameObject;
                buildingObjects.Add(child);
                originColor.Add(child.GetComponent<Renderer>().material.color);
            }
        }
    }

    /// <summary>
    /// color the buildings with data
    /// </summary>
    /// <param name="data">data contains V3 and Color of a building</param>
    public IEnumerator setBuildingColorContainData(string semanticName)
    {
        Dictionary<Vector3, Color> data = BuildingLoader.Instance.getSemanticData(semanticName);
        foreach (GameObject building in buildingObjects)
        {
            if (data.ContainsKey(building.GetComponent<BoxCollider>().center))
            {
                Renderer buildingRenderer = building.GetComponent<Renderer>();
                Color color = data[building.GetComponent<BoxCollider>().center];
                buildingRenderer.material.color = color;
            }
        }

        yield return null;
    }

    /// <summary>
    /// color the buildings with item installed with semantic data
    /// </summary>
    /// <param name="semanticName">semantic name of item that produce/consume</param>
    /// <returns></returns>
    public IEnumerator setBuildingColorContainInstalledItemWithSemanticData(string semanticName)
    {
        Dictionary<Vector3, Color> data = BuildingLoader.Instance.getSemanticData(semanticName);
        foreach (GameObject building in buildingObjects)
        {
            if (data.ContainsKey(building.GetComponent<BoxCollider>().center))
            {
                Dictionary<int, ShopItem> items = BuildingManager.Instance.GetBuildingState(building).GetItemsInstalled();
                foreach (KeyValuePair<int, ShopItem> item in items)
                {
                    foreach (var installedSemanticName in item.Value.semanticData)
                    {
                        if (installedSemanticName == semanticName)
                        {
                            Renderer buildingRenderer = building.GetComponent<Renderer>();
                            Color color = Settings.green;
                            buildingRenderer.material.color = color;
                        }
                    }
                }
            }
        }

        yield return null;
    }

    /// <summary>
    /// revert the color of the buildings
    /// </summary>
    public IEnumerator setBuildingColorContainData()
    {
        int i = 0;
        foreach (GameObject building in buildingObjects)
        {
            building.GetComponent<Renderer>().material.color = originColor[i];
            ++i;
        }

        yield return null;
    }
}
