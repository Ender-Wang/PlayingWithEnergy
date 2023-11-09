using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;
    public static string filePath = "Building/BuildingState";
    public Dictionary<string, Building> buildings { get; set; }
    public Dictionary<Building, GameObject> buildingObjects { get; set; }
    int totalEnergyProvision = 0;
    bool firstTime = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (Instance != this)
            Destroy(gameObject);

        StartCoroutine(LoadBuildings()); // load the building state file and initialize the buildings dictionary

    }

    /// <summary>
    /// Initialize the buildings dictionary
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadBuildings()
    {
        // load from the file
        buildings = ES3.Load<Dictionary<string, Building>>("buildings", filePath, new Dictionary<string, Building>());
        totalEnergyProvision = ES3.Load<int>("energyProvision", filePath, 0);
        buildingObjects = new Dictionary<Building, GameObject>();
        if (buildings.Count == 0)
        { // if the game is the first time running, create the buildings file
            buildings = new Dictionary<string, Building>();
            // find the buildings in the scene
            GameObject[] goes = GameObject.FindGameObjectsWithTag("Selectable");
            for (int i = 0; i < goes.Length; i++)
            {
                for (int j = 0; j < goes[i].transform.childCount; j++)
                {
                    var child = goes[i].transform.GetChild(j).gameObject;
                    if (child.CompareTag("ShopItem")) continue;
                    Building b = new Building(GetBuildingName(child));
                    float x = (child.GetComponent<BoxCollider>()?.size.x ?? 0) * 100;
                    float y = (child.GetComponent<BoxCollider>()?.size.y ?? 0) * 100;
                    float z = (child.GetComponent<BoxCollider>()?.size.z ?? 0) * 100;
                    b.SetSize(x, y, z);

                    buildings[GetBuildingName(child)] = b;
                    buildingObjects[b] = child;
                }
            }
            // ES3.Save("buildings", buildings, filePath);
        }
        else
        { // if the game is already running, fill in the dictionary of buildingsObject
            // Debug.Log("BuildingManager: LoadBuildings: buildings.Count: " + buildings.Count);
            firstTime = false;
            GameObject[] goes = GameObject.FindGameObjectsWithTag("Selectable");
            for (int i = 0; i < goes.Length; i++)
            {
                for (int j = 0; j < goes[i].transform.childCount; j++)
                {
                    var child = goes[i].transform.GetChild(j).gameObject;
                    buildingObjects[buildings[GetBuildingName(child)]] = child;
                }
            }
        }
        yield return null;
    }

    /// <summary>
    /// Update the energy provision/consumption of the buildings
    /// </summary>
    public void UpdateBuildingsEnergy()
    {
        if (totalEnergyProvision == 0)
        {
            foreach (KeyValuePair<Building, GameObject> kvp in buildingObjects)
            {
                // Update the energy provision/consumption according to the semantic data
                Dictionary<string, Dictionary<string, string>> buildingInfo = BuildingLoader.Instance.getBuildingInfo(kvp.Value);
                foreach (KeyValuePair<string, Dictionary<string, string>> semanticData in buildingInfo)
                {
                    // TODO: change the factor according to the semantic data (if needed)
                    float factor = 1;
                    int sign = 1;
                    foreach (KeyValuePair<string, string> data in semanticData.Value)
                    {
                        // info += " -" + data.Key + ": " + data.Value + "\n";
                        if (data.Key == "LevelFactor")
                        {
                            if (data.Value == "Energy Consumption")
                            {
                                factor = EnergyProcessBar.Instance.GetSemanticPercentage("Energy Consumption");
                                sign = -1;
                            }
                            else if (data.Value == "Energy Provision")
                            {
                                factor = EnergyProcessBar.Instance.GetSemanticPercentage("Energy Provision");
                                sign = 1;
                            }
                        }
                        else if (data.Key == "Contribution")
                        {
                            int contribution = int.Parse(data.Value);
                            if (sign == -1) // it's in consumption
                                contribution = DataSetting.getDataSetting(semanticData.Key).contributions.Count - contribution;
                            AddEnergy(sign, kvp.Key, Mathf.RoundToInt(contribution * factor));
                        }
                    }
                }
                totalEnergyProvision += Mathf.RoundToInt(kvp.Key.GetOldEnergyProvision());
            }
        }
    }

    public string GetBuildingName(GameObject building)
    {
        return building.transform.parent.parent.name + building.name; // e.g. Building_Neuperlach_Center  1
    }

    /// <summary>
    /// API: Add a building to the buildings dictionary
    /// </summary>
    /// <param name="newBuilding"></param>
    public void UpdateBuilding(Building newBuilding)
    {
        buildings[newBuilding.GetName()] = newBuilding;
    }

    /// <summary>
    /// API: Add (Remove) an item to the building
    /// </summary>
    /// <param name="buildingName"></param>
    /// <param name="shopItem"></param>
    /// <param name="remove"></param>
    public void UpdateBuilding(string buildingName, ShopItem shopItem, bool remove = false)
    {
        Building building = GetBuildingState(buildingName);
        if (remove)
        {
            building.RemoveItem(shopItem.id);
        }
        else
        {
            building.InstallItem(shopItem);
        }
    }

    /// <summary>
    /// API: Get the building state including building's level, item installation, etc.
    /// </summary>
    /// <param name="buildingName"></param>
    /// <returns></returns>
    public Building GetBuildingState(string buildingName)
    {
        if (buildings.ContainsKey(buildingName))
        {
            return buildings[buildingName];
        }
        else
        {
            Building newBuilding = new Building(buildingName);
            UpdateBuilding(newBuilding);
            return newBuilding;
        }
    }

    /// <summary>
    /// API: Get the building state including building's level, item installation, etc.
    /// </summary>
    /// <param name="building"></param>
    /// <returns></returns>
    public Building GetBuildingState(GameObject building)
    {
        string buildingName = GetBuildingName(building);
        if (buildings.ContainsKey(buildingName))
        {
            return buildings[buildingName];
        }
        else
        {
            Building newBuilding = new Building(buildingName);
            UpdateBuilding(newBuilding);
            return newBuilding;
        }
    }

    public void BuildingPrinter(string buildingName)
    {
        Building building = GetBuildingState(buildingName);
        Debug.Log(building.GetName());
        Debug.Log(building.GetTotalContribution());
        if (building.GetItemsInstalled().Count == 0)
        {
            Debug.Log("No item installed");
        }
        else
        {
            // foreach (ShopItem item in building.GetItemsInstalled())
            // {
            //     Debug.Log(item.name + "is installed");
            // }
        }

    }

    /// <summary>
    /// calculate the level of the building
    /// </summary>
    /// <param name="building"></param>
    /// <returns>level, rest of the contribution (experience)</returns>
    public KeyValuePair<int, int> GetBuildingLevel(Building building)
    {
        int level = 0;
        int contribution = building.GetTotalContribution();
        foreach (int boundary in Settings.buildingLevel)
        {
            if (contribution >= boundary)
            {
                level++;
                contribution -= boundary;
            }
            else
            {
                break;
            }
        }
        return new KeyValuePair<int, int>(level, contribution);
    }

    /// <summary>
    /// Add energy provision / consumption to the building according to the level factor
    /// </summary>
    /// <param name="building"></param>
    /// <param name="amount"></param>
    public void AddEnergy(LevelFactor levelFactor, GameObject building, int diff)
    {
        Building b = GetBuildingState(building);
        if (levelFactor == EnergyConsumption.Instance)
        {
            diff = -diff;
        }
        b.AddEnergyProvision(diff);
        totalEnergyProvision += diff;
    }

    public void AddEnergy(int sign, Building building, int diff)
    {
        building.AddEnergyProvision(diff * sign);
        totalEnergyProvision += diff * sign;
    }

    /// <summary>
    /// Get the total energy provision of the buildings
    /// </summary>
    /// <returns></returns>
    public int GetTotalEnergyProvision()
    {
        UpdateBuildingsEnergy();
        return totalEnergyProvision;
    }

    /// <summary>
    /// Save the building state to the file
    /// </summary>
    public void Save()
    {
        ES3.Save("buildings", buildings, filePath);
        ES3.Save("energyProvision", totalEnergyProvision, filePath);
    }
}
