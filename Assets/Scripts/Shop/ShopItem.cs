using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ES3Serializable, Serializable]
public class ShopItem
{
    [ES3Serializable]
    public int id { get; set; }
    [ES3Serializable]
    public string name { get; set; }
    [ES3Serializable]
    public List<string> category { get; set; }
    [ES3Serializable]
    public List<string> semanticData { get; set; }
    [ES3Serializable]
    public string description { get; set; }
    [ES3Serializable]
    private int price { get; set; }
    [ES3Serializable]
    public List<int> contribution { get; set; }
    [ES3Serializable]
    public string icon { get; set; }
    [ES3Serializable]
    public int currentLevel { get; set; }
    [ES3Serializable]
    public int maxLevel { get; set; }
    [ES3Serializable]
    public bool isVisible { get; set; }
    [ES3Serializable]
    public int upgradePrice { get; set; }
    [ES3Serializable]
    public List<int> maxContribution { get; set; }
    [ES3Serializable]
    public List<int> currentContributions { get; set; }
    [ES3Serializable]
    public float currentPrice { get; set; }
    [ES3Serializable]
    public float range { get; set; }
    [ES3Serializable]
    public List<Vector3> transform { get; set; }
    [ES3Serializable]
    public int numberOfItems { get; set; }

    // public ShopItem()
    // {
    //     name = "Default";
    //     category = "Default";
    //     description = "Default";
    //     price = 0;
    //     contribution = 0;
    //     icon = null;
    // }

    public ShopItem(int id, string name, List<string> category, List<string> semanticData, string description, int price, List<int> contribution, string icon, int upgradePrice, List<int> maxContribution)
    {
        this.id = id;
        this.name = name;
        this.category = category;
        this.semanticData = semanticData;
        this.description = description;
        this.price = price;
        currentPrice = price;
        this.contribution = contribution;
        currentContributions = new List<int>();
        foreach (int c in contribution)
        {
            currentContributions.Add(c);
        }
        this.icon = icon;
        this.maxLevel = maxLevel;
        currentLevel = 1;
        maxLevel = 1;
        isVisible = false;
        this.upgradePrice = upgradePrice;
        this.maxContribution = maxContribution;
    }

    public ShopItem(int id, string name, List<string> category, List<string> semanticData, string description, int price, List<int> contribution, string icon, int upgradePrice, List<int> maxContribution, float range)
    {
        this.id = id;
        this.name = name;
        this.category = category;
        this.semanticData = semanticData;
        this.description = description;
        this.price = price;
        currentPrice = price;
        this.contribution = contribution;
        currentContributions = new List<int>();
        foreach (int c in contribution)
        {
            currentContributions.Add(c);
        }
        this.icon = icon;
        this.maxLevel = maxLevel;
        currentLevel = 1;
        maxLevel = 1;
        isVisible = false;
        this.upgradePrice = upgradePrice;
        this.maxContribution = maxContribution;
        this.range = range;
        numberOfItems = 1;
    }

    public ShopItem(int id, string name, List<string> category, List<string> semanticData, string description, int price, List<int> contribution, string icon, int maxLevel, int currentLevel, bool isVisible, int upgradePrice, List<int> maxContribution, List<int> currentContributions, float currentPrice, float range, List<Vector3> transform, int numberOfItems)
    {
        this.id = id;
        this.name = name;
        this.category = category;
        this.semanticData = semanticData;
        this.description = description;
        this.price = price;
        this.contribution = contribution;
        this.currentContributions = currentContributions;
        this.currentPrice = currentPrice;
        this.icon = icon;
        this.maxLevel = maxLevel;
        this.currentLevel = currentLevel;
        this.isVisible = isVisible;
        this.upgradePrice = upgradePrice;
        this.maxContribution = maxContribution;
        this.range = range;
        this.transform = transform;
        this.numberOfItems = numberOfItems;
    }

    public ShopItem Install()
    {
        return new ShopItem(id, name, category, semanticData, description, price, contribution, icon, maxLevel, currentLevel, isVisible, upgradePrice, new List<int>(maxContribution), new List<int>(currentContributions), currentPrice, range, transform, numberOfItems);
    }

    public ShopItem Install(int numberOfItem)
    {
        return new ShopItem(id, name, category, semanticData, description, price, contribution, icon, maxLevel, currentLevel, isVisible, upgradePrice, new List<int>(maxContribution), new List<int>(currentContributions), currentPrice, range, transform, numberOfItem);
    }

    public void UpgradeTo(int level)
    {
        currentPrice += (level - currentLevel) * upgradePrice;
        for (int i = 0; i < currentContributions.Count; i++)
        {
            int upgradedContribution = (maxContribution[i] - contribution[i]) / (SkillManager.Instance.getUserSkills()[name].getMaxLevel() - 1);
            if (currentContributions[i] + (level - currentLevel) * upgradedContribution <= maxContribution[i])
                currentContributions[i] += (level - currentLevel) * upgradedContribution;
            else
                Debug.LogError("The contribution of " + semanticData[i] + " is out of range!");
        }
        currentLevel = level;
    }

    /// <summary>
    /// Create a new shopItem with the same information as this shopItem, but with the level of the parameter
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public ShopItem LevelPreview(int level)
    {
        ShopItem item = Install();
        item.UpgradeTo(level);
        return item;
    }

    public void ResetItem()
    {
        currentLevel = 1;
        currentPrice = price;
        for (int i = 0; i < currentContributions.Count; i++)
        {
            currentContributions[i] = contribution[i];
        }
    }

    /// <summary>
    /// shopItem.transform.position, shopItem.transform.rotation.eulerAngles, shopItem.transform.localScale
    /// </summary>
    /// <returns></returns>
    public List<Vector3> GetTransform()
    {
        return transform;
    }

    /// <summary>
    /// Only compare position, rotation and scale with single infrastructure
    /// </summary>
    /// <param name="building"></param>
    /// <returns></returns>
    public bool CompareTo(Transform building)
    {
        Vector3 dic = new Vector3(transform[0].x, 0, transform[0].z);
        Vector3 b = new Vector3(building.position.x, 0, building.position.z);
        float distance = Vector3.Distance(dic, b);
        return distance <= 0.01 && building.rotation.eulerAngles == transform[1] && building.localScale == transform[2];
    }

}
