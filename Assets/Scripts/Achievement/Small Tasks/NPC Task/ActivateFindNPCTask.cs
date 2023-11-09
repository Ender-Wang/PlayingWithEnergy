using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.AI;

public class ActivateFindNPCTask
{

    //Class structure used for deserializing taskSpecification from string to JSON
    [System.Serializable]
    public class Object
    {
        public string name;
        public string subTaskTitle;
        public List<float> location;
    }

    [System.Serializable]
    public class TaskNPC
    {
        public string name;
        public string subTaskTitle;
        public List<float> location;
        public List<Object> objects;
        public List<Object> npcs;
    }
    [System.Serializable]
    public class TaskSpecRoot
    {
        public List<int> round;
        public List<TaskNPC> npcs;
    }

    /// <summary>
    /// Initiate NPC Task with taskID, airdropLocation, taskSpecificationData, coinRewardAmount, levelFactorPointRewardAmount.
    /// </summary>
    /// <param name="taskID"> Task ID of the current task.</param>
    /// <param name="airdropLocation"></param>
    /// <param name="taskSpecificationData"></param>
    /// <param name="coinRewardAmount"></param>
    /// <param name="levelFactorPointRewardAmount"></param>
    public void InitNPCTaskN(int taskID, Vector3 airdropLocation, string taskSpecificationData, int coinRewardAmount, int levelFactorPointRewardAmount)
    {
        var taskSpecification = JsonUtility.FromJson<TaskSpecRoot>(taskSpecificationData);
        TrackingManager trackingManager = GameObject.FindObjectsOfType<TrackingManager>()[0];
        string playerName = "";

        // init task object
        string taskName = "Task-" + taskID.ToString();
        GameObject taskObject = new GameObject(taskName);
        taskObject.AddComponent(typeof(Task));
        Task task = taskObject.GetComponent<Task>();
        task.taskName = taskName;
        task.taskID = taskID;
        task.coinRewardAmount = coinRewardAmount;
        task.levelFactorPointRewardAmount = levelFactorPointRewardAmount;

        // change the position of the player
        Transform player = GameObject.FindWithTag("Player").transform;
        task.playerPositionRecord = player.position;
        player.position = airdropLocation;

        // Add NPCs
        List<GameObject> npcPrefabs = new List<GameObject>(Resources.LoadAll<GameObject>("Achievement Resources/Small Tasks/NPCs"));

        for (int i = 0; i < taskSpecification.npcs.Count; i++)
        {
            // Subtask
            // init NPC
            GameObject prefab = npcPrefabs.Find(npcPrefab => npcPrefab.name == taskSpecification.npcs[i].name);
            GameObject newNPC;
            Vector3 pos = new Vector3(taskSpecification.npcs[i].location[0], taskSpecification.npcs[i].location[1], taskSpecification.npcs[i].location[2]);
            newNPC = GameObject.Instantiate(prefab);
            newNPC.transform.position = pos;
            newNPC.transform.parent = taskObject.transform;
            newNPC.name = "[" + taskSpecification.npcs[i].name + "]"; // TODO: mark the NPC as the header of chain of subsubtasks
            GameObject.Destroy(newNPC.GetComponent<NPCRandomMoving>());
            GameObject.Destroy(newNPC.GetComponent<NavMeshAgent>());
            newNPC.AddComponent(typeof(NPC));
            NPC npc = newNPC.GetComponent<NPC>();
            npc.Show(FollowingType.Mark);
            trackingManager.AddBinnacleTrackedObjectScript(newNPC, TrackingManager.MarkerType.NPC);

            //-------------- add task component
            newNPC.AddComponent(typeof(Task));
            Task subTask = newNPC.GetComponent<Task>();
            task.AddSubTask(subTask);
            subTask.taskName = taskSpecification.npcs[i].subTaskTitle.Split('|')[0];
            playerName = taskSpecification.npcs[i].subTaskTitle.Split('|')[1];

            bool isFindObjTask = taskSpecification.npcs[i].objects != null && taskSpecification.npcs[i].objects.Count != 0;
            bool isFindNPCTask = taskSpecification.npcs[i].npcs != null && taskSpecification.npcs[i].npcs.Count != 0;

            //--------------

            if (isFindObjTask)
            {
                for (int k = 0; k < taskSpecification.npcs[i].objects.Count; k++)
                {
                    Vector3 objectLocation = new Vector3(taskSpecification.npcs[i].objects[k].location[0], taskSpecification.npcs[i].objects[k].location[1], taskSpecification.npcs[i].objects[k].location[2]);
                    GameObject newObject = GameObject.Instantiate(Resources.Load<GameObject>("Achievement Resources/Small Tasks/Objs/" + taskSpecification.npcs[i].objects[k].name));
                    newObject.transform.position = objectLocation;
                    newObject.transform.parent = newNPC.transform;
                    newObject.SetActive(false);
                    newObject.name = taskSpecification.npcs[i].objects[k].name;
                    newObject.transform.localScale = new Vector3(8, 8, 8);
                    trackingManager.AddBinnacleTrackedObjectScript(newObject, TrackingManager.MarkerType.Object);
                    newObject.AddComponent(typeof(Obj));

                    // init task object
                    newObject.AddComponent(typeof(Task));
                    Task subsubTask = newObject.GetComponent<Task>();
                    subTask.AddSubTask(subsubTask);
                    subsubTask.taskName = taskSpecification.npcs[i].objects[k].subTaskTitle;
                }
            }

            if (isFindNPCTask)
            {
                for (int k = 0; k < taskSpecification.npcs[i].npcs.Count; k++)
                {
                    Vector3 npcLocation = new Vector3(taskSpecification.npcs[i].npcs[k].location[0], taskSpecification.npcs[i].npcs[k].location[1], taskSpecification.npcs[i].npcs[k].location[2]);
                    GameObject newNPC2 = GameObject.Instantiate(Resources.Load<GameObject>("Achievement Resources/Small Tasks/NPCs/" + taskSpecification.npcs[i].npcs[k].name));
                    newNPC2.transform.position = npcLocation;
                    newNPC2.transform.parent = newNPC.transform;
                    GameObject.Destroy(newNPC2.GetComponent<NPCRandomMoving>());
                    GameObject.Destroy(newNPC2.GetComponent<NavMeshAgent>());
                    newNPC2.SetActive(false);
                    trackingManager.AddBinnacleTrackedObjectScript(newNPC2, TrackingManager.MarkerType.SubTaskNPC);
                    newNPC2.AddComponent(typeof(NPC));
                    newNPC2.AddComponent(typeof(Task));
                    newNPC2.transform.localScale = new Vector3(100f, 100f, 100f);
                    Task subsubTask = newNPC2.GetComponent<Task>();
                    subTask.AddSubTask(subsubTask);
                    subsubTask.taskName = taskSpecification.npcs[i].npcs[k].subTaskTitle;
                    // add dialog component
                    newNPC2.AddComponent(typeof(CharacterDialogSystem));
                    newNPC2.GetComponent<CharacterDialogSystem>().Initialize(TaskType.Other, taskID, newNPC2.name, 1, new List<string>() { newNPC.name });
                }
            }

            //--------------- dialog creation
            // initialize the objects name
            List<string> objectsName = null;
            if (taskSpecification.npcs[i].objects != null && taskSpecification.npcs[i].objects.Count != 0)
            {
                objectsName = new List<string>();
                foreach (Object obj in taskSpecification.npcs[i].objects)
                {
                    objectsName.Add(obj.name);
                }
            }
            List<string> NPCsName = null;
            if (taskSpecification.npcs[i].npcs != null && taskSpecification.npcs[i].npcs.Count != 0)
            {
                NPCsName = new List<string>();
                foreach (Object npc2 in taskSpecification.npcs[i].npcs)
                {
                    NPCsName.Add(npc2.name);
                }
            }
            TaskType taskType = isFindObjTask ? TaskType.FindObjects : TaskType.FindNPC;
            int r = taskSpecification.round[i];
            // add dialog component
            newNPC.AddComponent(typeof(CharacterDialogSystem));
            newNPC.GetComponent<CharacterDialogSystem>().Initialize(taskType, taskID, newNPC.name, r, isFindObjTask ? objectsName : NPCsName);
            // subTaskObject.SetActive(i == 0 ? true : false);

            if (i != 0)
                newNPC.SetActive(false);
        }

        GameObject taskList = Resources.Load<GameObject>("Achievement Resources/Small Tasks/TaskList");
        taskList = GameObject.Instantiate(taskList, GameManager.Instance.canvas.transform);
        taskList.GetComponent<TaskListController>().SetTaskList(task);

        //Change player's character
        var character = player.transform.Find("Character").GetChild(0).gameObject;
        GameObject oldCharacter = character;
        // Debug.Log("Player Name: " + playerName);
        character = GameObject.Instantiate(Resources.Load<GameObject>("Achievement Resources/Small Tasks/NPCs/" + playerName), character.transform.parent);
        character.name = character.name.Replace("(Clone)", "");
        character.transform.localScale = oldCharacter.transform.localScale;
        character.transform.localPosition = oldCharacter.transform.localPosition;
        character.transform.localRotation = oldCharacter.transform.localRotation;
        GameObject.Destroy(character.GetComponent<NPCRandomMoving>());
        GameObject.Destroy(character.GetComponent<NavMeshAgent>());
        GameObject.Destroy(oldCharacter);
    }
}
