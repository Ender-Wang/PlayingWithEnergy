using UnityEngine;
using System;

[ES3Serializable, Serializable]
public class TaskItem
{
    [ES3Serializable]
    public int achievementID { get; set; } // achievement id indicates which achievement this task belongs to
    [ES3Serializable]
    public int taskID { get; set; }
    [ES3Serializable]
    public bool status { get; set; } // true = completed, false = not completed
    [ES3Serializable]
    public string title { get; set; }
    [ES3Serializable]
    public string description { get; set; }
    [ES3Serializable]
    public Vector3[] taskLocation { get; set; } // n possible locations for each task, randomly pick one
    [ES3Serializable]
    public int timeLimit { get; set; } // task time limit in minutes
    [ES3Serializable]
    public string difficulty { get; set; }
    [ES3Serializable]
    public int coinReward { get; set; }
    [ES3Serializable]
    public int levelFactorPointReward { get; set; }

    public TaskItem()
    {
        achievementID = 0;
        taskID = 0;
        status = false;
        title = "Default";
        description = "Default";
        taskLocation = new Vector3[3];
        timeLimit = 0;
        difficulty = "Default";
        coinReward = 0;
        levelFactorPointReward = 0;
    }

    public TaskItem(int achievementID, int taskID, bool status, string title, string description, Vector3[] taskLocation, int timeLimit, string difficulty, int coinReward, int levelFactorPointReward)
    {
        this.achievementID = achievementID;
        this.taskID = taskID;
        this.status = status;
        this.title = title;
        this.description = description;
        this.taskLocation = taskLocation;
        this.timeLimit = timeLimit;
        this.difficulty = difficulty;
        this.coinReward = coinReward;
        this.levelFactorPointReward = levelFactorPointReward;
    }
}