using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TaskPool : MonoBehaviour
{
    /// <summary>
    /// Find OldDevice xxx in area(building) xxx
    /// </summary>
    /// 
    [Header("Task Pool Test")]
    [Tooltip("Areas(buildings) contains old devices")]
    public GameObject[] areas;
    [Tooltip("Old devices")]
    public GameObject[] oldDevices;

    // Start is called before the first frame update
    void Start()
    {
        generateTask();
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void generateTask()
    {
        Dictionary<string, string[]> locationItems = new Dictionary<string, string[]>()
        {
            { "Supermarket", new string[] { "Light bulbs", "Thermostats", "Refrigerators", "Freezers", "Heaters" } },
            { "Gas Station", new string[] { "Fuel pumps", "Fuel dispensers", "Storage tanks", "Nozzles", "Hoses" } },
            { "Office Building", new string[] { "Air conditioners", "Lighting systems", "Elevators", "Boilers", "Water heaters" } },
            { "Factory", new string[] { "Furnaces", "Boilers", "Heat exchangers", "Compressors", "Pumps" } },
            { "Data Center", new string[] { "Servers", "Air conditioners", "Power distribution units", "Uninterruptible power supplies", "Generators" } }
        };

        string[] locations = locationItems.Keys.ToArray();
        string randomLocation = locations[UnityEngine.Random.Range(0, locations.Length)];

        string[] items = locationItems[randomLocation];
        string randomItem = items[UnityEngine.Random.Range(0, items.Length)];

        string task = $"Find {randomItem} in {randomLocation}";

        Debug.Log(task);
    }
}
