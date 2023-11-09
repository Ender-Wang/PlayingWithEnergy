using UnityEngine;
using System.Collections.Generic;

public class TaskSpecificationManager : MonoBehaviour
{
    public TextAsset taskSpecificationJSON;
    public TextAsset nameLocationJSON;
    public static TaskSpecificationManager Instance;

    //Task Specification Classes
    [System.Serializable]
    public class Object
    {
        public string name;
        public string subTaskTitle;
        public List<float> location;
    }

    [System.Serializable]
    public class NPC
    {
        public string name;
        public string subTaskTitle;
        public List<float> location;
        public List<Object> objects;
        public List<Object> npcs;
    }

    [System.Serializable]
    public class Option
    {
        public List<int> round;
        public List<NPC> npcs;
    }

    [System.Serializable]
    public class TaskSpecification
    {
        public int taskID;
        public List<Option> options;
    }

    [System.Serializable]
    public class TaskSpecificationRoot
    {
        public List<TaskSpecification> taskSpecification;
    }

    //====================================================================================================
    //Name Location Classes
    [System.Serializable]
    public class Location
    {
        public List<float> location;
    }
    [System.Serializable]
    public class NameLocation
    {
        public List<string> NPC;
        public List<string> Object;
        public List<Location> Location;
    }

    [System.Serializable]
    public class NameLocationRoot
    {
        public NameLocation nameLocation;
    }

    private void Start()
    {
        // GetTaskSpecification(1);
    }
    private void Awake()
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

    public string GetTaskSpecification(int targetTaskID)
    {
        var taskSpecificationData = JsonUtility.FromJson<TaskSpecificationRoot>(taskSpecificationJSON.text);
        var nameLocationData = JsonUtility.FromJson<NameLocationRoot>(nameLocationJSON.text);

        TaskSpecification targetTask = taskSpecificationData.taskSpecification.Find(task => task.taskID == targetTaskID);

        // Get random option
        int randomOptionIndex = Random.Range(0, targetTask.options.Count);
        Option randomOption = targetTask.options[randomOptionIndex];

        //Randomize the round based on NPC count
        // int npcsCount = randomOption.npcs != null ? randomOption.npcs.Count : 0;
        // List<int> randomRound = new List<int>(npcsCount);
        // for (int i = 0; i < npcsCount; i++)
        // {
        //     //Define the round. First round: find all objects; second round: finish the task
        //     randomRound.Add(2);
        // }
        // randomOption.round = randomRound;

        //1. Get NPC name, subTaskTitle, random assign location
        //2. Get Object name, subTaskTitle, random assign location
        foreach (var npc in randomOption.npcs)
        {
            // Debug.Log("NPC name:" + npc.name);
            // Debug.Log("NPC task title:" + npc.subTaskTitle);
            npc.location = GetRandomLocation();

            if (npc.objects.Count > 0)
            {
                foreach (var obj in npc.objects)
                {
                    // Debug.Log("Object name:" + obj.name);
                    // Debug.Log("Object subtask title:" + obj.subTaskTitle);
                    obj.location = GetRandomLocation();
                }
            }

            if (npc.npcs.Count > 0)
            {
                foreach (var npc2 in npc.npcs)
                {
                    // Debug.Log("Sub-NPC name:" + npc2.name);
                    // Debug.Log("Sub-NPC subtask title:" + npc2.subTaskTitle);
                    npc2.location = GetRandomLocation();
                }
            }
        }

        //Get random location from Location list
        List<float> GetRandomLocation()
        {
            int randomLocationIndex = Random.Range(0, nameLocationData.nameLocation.Location.Count);
            List<float> randomLocation = new List<float>(3);
            randomLocation.Add(nameLocationData.nameLocation.Location[randomLocationIndex].location[0]);
            randomLocation.Add(nameLocationData.nameLocation.Location[randomLocationIndex].location[1]);
            randomLocation.Add(nameLocationData.nameLocation.Location[randomLocationIndex].location[2]);
            return randomLocation;
        }

        //Stringify the random option and pass it
        string randomOptionString = JsonUtility.ToJson(randomOption);
        Debug.Log($"TaskID: {targetTask.taskID}. Options content: {randomOptionString}");
        return randomOptionString;
    }
}
