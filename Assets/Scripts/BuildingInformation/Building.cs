using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    [ES3Serializable]
    private string m_name;
    [ES3Serializable]
    private Dictionary<int, ShopItem> m_itemsInstalled;
    [ES3Serializable]
    private Dictionary<string, int> m_contributions;
    [ES3Serializable]
    private Vector3 m_size;
    [ES3Serializable]
    private float m_totalEnergyProvision;

    public Building(string name)
    {
        m_name = name;
        m_itemsInstalled = new Dictionary<int, ShopItem>();
        m_contributions = new Dictionary<string, int>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="size"></param>
    public void SetSize(Vector3 size)
    {
        m_size = size;
    }

    public void SetSize(float x, float y, float z)
    {
        Vector3 size = new Vector3(x, y, z);
        m_size = size;
    }

    public string GetName()
    {
        return m_name;
    }

    /// <summary>
    /// Get the size of the building
    /// </summary>
    /// <returns></returns>
    public Vector3 GetSize() { return m_size; }

    public void SetName(string name)
    {
        m_name = name;
    }

    public Dictionary<string, int> GetContributions()
    {
        return m_contributions;
    }

    /// <summary>
    /// Calculate the total contribution of the building
    /// </summary>
    /// <returns></returns>
    public int GetTotalContribution()
    {
        int total = 0;
        foreach (KeyValuePair<string, int> contribution in m_contributions)
        {
            total += contribution.Value;
        }
        // Plus all the default semantic datas
        Dictionary<string, Dictionary<string, string>> buildingInfo = BuildingLoader.Instance.getBuildingInfo(BuildingManager.Instance.buildingObjects[this]);
        // string info = "";
        foreach (KeyValuePair<string, Dictionary<string, string>> sematicData in buildingInfo)
        {
            // info += "Semantic: " + sematicData.Key + "\n";
            foreach (KeyValuePair<string, string> data in sematicData.Value)
            {
                // info += " -" + data.Key + ": " + data.Value + "\n";
                if (data.Key == "Contribution")
                {
                    total += int.Parse(data.Value) * 100;
                }
            }
        }
        // Debug.Log(info);
        return total;
    }

    public Dictionary<int, ShopItem> GetItemsInstalled()
    {
        return m_itemsInstalled;
    }

    /// <summary>
    /// Install the item to the building and update the semantic data
    /// </summary>
    /// <param name="item"></param>
    public void InstallItem(ShopItem item)
    {
        if (m_itemsInstalled.ContainsKey(item.id))
        {
            ShopItem existingItem = m_itemsInstalled[item.id];
            for (int i = 0; i < existingItem.semanticData.Count; i++)
            { // update all semantic data
                int contribution = 0;
                if (m_contributions.ContainsKey(existingItem.semanticData[i]))
                { // if semantic data already exists
                    contribution = m_contributions[existingItem.semanticData[i]];
                    contribution -= existingItem.currentContributions[i]; // subtract contribution from previous item
                }
                m_contributions[existingItem.semanticData[i]] = contribution + item.currentContributions[i];
                updateSemanticDataWithDiff(existingItem.semanticData[i], BuildingManager.Instance.buildingObjects[this], m_contributions[existingItem.semanticData[i]]);
                // if (m_contributions[existingItem.semanticData[i]] >= 100)
                // { // if contribution is greater than 100%, upgade semantic data
                //     int buffer = m_contributions[existingItem.semanticData[i]] / 100;

                //     m_contributions[existingItem.semanticData[i]] -= 100 * buffer;
                // }
            }
            m_itemsInstalled[item.id] = item;
        }
        else
        {
            for (int i = 0; i < item.semanticData.Count; i++)
            { // update all semantic data
                int contribution = 0;
                if (m_contributions.ContainsKey(item.semanticData[i]))
                { // if semantic data already exists
                    contribution = m_contributions[item.semanticData[i]];
                    contribution -= item.currentContributions[i]; // subtract contribution from previous item
                }
                m_contributions[item.semanticData[i]] = contribution + item.currentContributions[i];
                updateSemanticDataWithDiff(item.semanticData[i], BuildingManager.Instance.buildingObjects[this], m_contributions[item.semanticData[i]]);
                // if (m_contributions[item.semanticData[i]] >= 100)
                // { // if contribution is greater than 100%, upgade semantic data
                //     int buffer = m_contributions[item.semanticData[i]] / 100;
                //     updateSemanticDataWithDiff(item.semanticData[i], BuildingManager.Instance.buildingObjects[this], buffer);
                //     m_contributions[item.semanticData[i]] -= 100 * buffer;
                //     Debug.Log("Buffer: " + buffer);
                // }
            }
            m_itemsInstalled.Add(item.id, item);
        }
    }

    public void RemoveItem(int id)
    {
        m_itemsInstalled.Remove(id);
    }

    //TODO: JIAJING YI
    void updateSemanticDataWithDiff(string semanticData, GameObject building, int diff)
    {
        BuildingLoader.Instance.updateSemanticDataWithDiff(semanticData, building, diff);
    }

    /// <summary>
    /// Set the total energy provision of the building
    /// </summary>
    /// <param name="totalEnergyProvision"></param>
    public void SetTotalEnergyProvision(float totalEnergyProvision)
    {
        m_totalEnergyProvision = totalEnergyProvision;
    }

    /// <summary>
    /// Add the energy provision to the building
    /// </summary>
    /// <param name="energyProvision"></param>
    public void AddEnergyProvision(float energyProvision)
    {
        m_totalEnergyProvision += (energyProvision * (m_size.x * m_size.y * m_size.z) / Settings.energyDivisor);
    }

    public float GetTotalEnergyProvision()
    {
        return m_totalEnergyProvision + GetOldEnergyProvision();
    }

    public float GetOldEnergyProvision()
    {
        return Settings.oldEnergyProvision * ((m_size.x * m_size.y * m_size.z) / Settings.energyDivisor);
    }

}
