using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingLoader
{
    public static BuildingLoader Instance = new BuildingLoader();

    // key: semantic name
    private static Dictionary<string, DataSetting> DataSettings;
    private static Dictionary<string, Dictionary<Vector3, Color>> Datas;

    BuildingLoader()
    {
        loadDatas();
    }

    public void loadDatas()
    {
        // load all the data settings
        DataSettings = DataSetting.getDataSettings();

        // load all the data
        Datas = new Dictionary<string, Dictionary<Vector3, Color>>();
        foreach (string semanticName in DataSettings.Keys)
        {
            Datas.Add(semanticName, ES3.Load<Dictionary<Vector3, Color>>(semanticName, "Building/SemanticData/" + semanticName, new Dictionary<Vector3, Color>()));
        }
    }

    public void store()
    {
        foreach (KeyValuePair<string, Dictionary<Vector3, Color>> kvp in Datas)
        {
            ES3.Save<Dictionary<Vector3, Color>>(kvp.Key, kvp.Value, "Building/SemanticData/" + kvp.Key);
        }
    }

    public Dictionary<Vector3, Color> getSemanticData(string semanticName)
    {
        if (Datas.ContainsKey(semanticName))
        {
            return Datas[semanticName];
        }
        else
        {
            Debug.Log("No such semantic data: " + semanticName);
            return new Dictionary<Vector3, Color>();
        }
    }

    public LevelFactor getLevelFactor(string semanticName)
    {
        if (DataSettings.ContainsKey(semanticName))
        {
            string name = DataSettings[semanticName].levelFactor;
            return LevelManager.getLevelFactor(name);
        }
        else
        {
            Debug.Log("No such semantic data: " + semanticName);
            return LevelManager.getLevelFactor("Default");
        }
    }

    public Dictionary<string, Dictionary<string, string>> getBuildingInfo(GameObject building)
    {
        Dictionary<string, Dictionary<string, string>> buildingInfo = new Dictionary<string, Dictionary<string, string>>();
        Vector3 positon = building.GetComponent<BoxCollider>().center;

        foreach (KeyValuePair<string, DataSetting> setting in DataSettings)
        {
            DataSetting dataSetting = setting.Value;
            string semanticName = setting.Key;
            string displayedTitle = dataSetting.displayedTitle;
            try
            {
                Color color = getOrInitColor(semanticName, positon);
                int index = dataSetting.colors.IndexOf(color); // you can use the index to get the corresponding info from DataSetting

                // build the building info of this semantic
                Dictionary<string, string> semanticDataOfBuilding = new Dictionary<string, string>();
                semanticDataOfBuilding.Add(displayedTitle, dataSetting.displayedInfos[index]);
                semanticDataOfBuilding.Add("LevelFactor", dataSetting.levelFactor);
                semanticDataOfBuilding.Add("Contribution", dataSetting.contributions[index].ToString());
                buildingInfo.Add(semanticName, semanticDataOfBuilding);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        //printInfo(positon, buildingInfo); // For test

        return buildingInfo;
    }

    private void printInfo(Vector3 position, Dictionary<string, Dictionary<string, string>> buildingInfo)
    {
        string info = "Building Info: " + position + "\n";
        foreach (KeyValuePair<string, Dictionary<string, string>> sematicData in buildingInfo)
        {
            info += "Semantic: " + sematicData.Key + "\n";
            foreach (KeyValuePair<string, string> data in sematicData.Value)
            {
                info += " -" + data.Key + ": " + data.Value + "\n";
            }
        }
        Debug.Log(info);
    }

    public void updateSemanticData(string semanticName, GameObject building, int contribution)
    {
        var position = building.GetComponent<BoxCollider>().center;

        DataSetting dataSetting = DataSettings[semanticName];

        // get the index of the old value and new value
        Color oldColor = getOrInitColor(semanticName, position);
        int oldIndex = dataSetting.colors.IndexOf(oldColor);
        int newIndex = dataSetting.contributions.IndexOf(contribution);

        if (newIndex == -1)
        {
            Debug.Log("Already maxmium Contribution");
            newIndex = dataSetting.contributions.IndexOf(dataSetting.contributions.Max());
        }
        Color newColor = dataSetting.colors[newIndex];

        // update the level factor
        updateLevelFactor(dataSetting.contributions[oldIndex], dataSetting.contributions[newIndex], dataSetting.levelFactor);
        // update the energy increase rate
        BuildingManager.Instance.AddEnergy(getLevelFactor(semanticName), building, contribution - dataSetting.contributions[oldIndex]);

        // update the energy process bar
        EnergyProcessBar.Instance.UpdateEnergy(getLevelFactor(semanticName), semanticName, contribution - dataSetting.contributions[oldIndex]);

        // update the total contribution
        dataSetting.totalContributions += contribution - dataSetting.contributions[oldIndex];

        // update the semantic data
        Datas[semanticName][position] = newColor;

    }

    public void updateSemanticDataWithDiff(string semanticName, GameObject building, int diff)
    {
        if (diff == 0) return;
        var position = building.GetComponent<BoxCollider>().center;
        DataSetting dataSetting = DataSettings[semanticName];
        // get the index of the old value and new value
        Color oldColor = getOrInitColor(semanticName, position);
        int oldIndex = dataSetting.colors.IndexOf(oldColor);
        int oldContribution = dataSetting.contributions[oldIndex];
        int newContribution = oldContribution + diff;
        int newIndex = dataSetting.contributions.IndexOf(newContribution);

        if (newIndex < 0)
        {
            Debug.Log("Already maxmium Contribution");
            newIndex = dataSetting.contributions.IndexOf(dataSetting.contributions.Max());
        }

        Color newValue = dataSetting.colors[newIndex];

        // update the level factor
        updateLevelFactor(dataSetting.contributions[oldIndex], dataSetting.contributions[newIndex], dataSetting.levelFactor);
        // update the energy increase rate
        BuildingManager.Instance.AddEnergy(getLevelFactor(semanticName), building, diff);
        // update the energy process bar
        EnergyProcessBar.Instance.UpdateEnergy(getLevelFactor(semanticName), semanticName, diff);
        // update the total contribution
        dataSetting.totalContributions += dataSetting.contributions[newIndex] - dataSetting.contributions[oldIndex];
        // update the semantic data
        Datas[semanticName][position] = newValue;

    }

    private void updateLevelFactor(int oldLevelPoint, int contribution, string levelFactorStr)
    {
        LevelFactor levelFactor = LevelManager.getLevelFactor(levelFactorStr);
        levelFactor.update(oldLevelPoint, contribution);
    }

    private Color getOrInitColor(string semanticName, Vector3 position)
    {
        // fuzzy matches
        // var filteredKVPair = picDatas[semanticName].Where(kvp => MyVector3Comparer.Instance.Equals(kvp.Key, position)).First();

        if (!Datas[semanticName].ContainsKey(position))
        {
            return initBuilding(semanticName, position);
        }
        return Datas[semanticName][position];
    }

    // init the building with the default value in data setting
    private Color initBuilding(string semanticName, Vector3 position)
    {
        Debug.Log("No such semantic data: " + semanticName + " at " + position + ", init with default value");
        var DataSetting = DataSettings[semanticName];
        int index = DataSetting.contributions.IndexOf(0);
        Datas[semanticName][position] = DataSetting.colors[index];
        return DataSetting.colors[index];
    }

}
