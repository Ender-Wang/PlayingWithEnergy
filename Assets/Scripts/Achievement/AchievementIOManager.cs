using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementIOManager
{
    private Dictionary<int, TaskItem> taskItemsTemp;

    //Achievement ES3 initialize from Achievement CSV
    public static void LoadAchievementRegister()
    {
        Dictionary<int, AchievementItem> achievementItemsES3 = ES3.Load<Dictionary<int, AchievementItem>>("Achievement", "Achievement/Achievement", new Dictionary<int, AchievementItem>());
        if (achievementItemsES3.Count == 0)
        {
            foreach (AchievementItem achievement in AchievementBoardManager.Instance.achievementItems)
            {
                if (!achievementItemsES3.ContainsKey(achievement.achievementID))
                {
                    achievementItemsES3.Add(achievement.achievementID, achievement);
                }
            }
            ES3.Save<Dictionary<int, AchievementItem>>("Achievement", achievementItemsES3, "Achievement/Achievement");
            Debug.Log("Achievement Record Initialized from CSV");
        }
    }

    //Task ES3 initialize from Task CSV
    public static void LoadTaskRegister()
    {
        Dictionary<int, TaskItem> taskItemsES3 = ES3.Load<Dictionary<int, TaskItem>>("Task", "Achievement/Task", new Dictionary<int, TaskItem>());
        if (taskItemsES3.Count == 0)
        {
            foreach (TaskItem task in TaskManager.Instance.taskItems)
            {
                if (!taskItemsES3.ContainsKey(task.taskID))
                {
                    taskItemsES3.Add(task.taskID, task);
                }
            }
            ES3.Save<Dictionary<int, TaskItem>>("Task", taskItemsES3, "Achievement/Task");
            Debug.Log("Task Record Initialized from CSV");
        }
    }

    //Update completed Task record in ES3
    public static void updateTaskRegister(int taskID)
    {
        Dictionary<int, TaskItem> taskItemsES3 = ES3.Load<Dictionary<int, TaskItem>>("Task", "Achievement/Task", new Dictionary<int, TaskItem>());
        foreach (KeyValuePair<int, TaskItem> task in taskItemsES3)
        {
            if (task.Key == taskID)
            {
                task.Value.status = true;
                break;
            }
        }
        ES3.Save<Dictionary<int, TaskItem>>("Task", taskItemsES3, "Achievement/Task");
    }

    //Update completed Achievement record in ES3
    public static void updateAchievementRegister(int achievementID)
    {
        Dictionary<int, AchievementItem> achievementItemsES3 = ES3.Load<Dictionary<int, AchievementItem>>("Achievement", "Achievement/Achievement", new Dictionary<int, AchievementItem>());
        foreach (KeyValuePair<int, AchievementItem> achievement in achievementItemsES3)
        {
            if (achievement.Key == achievementID)
            {
                achievement.Value.status = true;
                break;
            }
        }
        ES3.Save<Dictionary<int, AchievementItem>>("Achievement", achievementItemsES3, "Achievement/Achievement");
    }

    /// <summary>
    /// Get AchievementID, achievementCoinReward, achievementLevelFactorPointReward by TaskID
    /// </summary>
    /// <param name="taskID">TaskID</param>
    /// <returns>int[] achievementInfo: [achievementID, coinReward, levelFactorPointReward]</returns>
    public static int[] getAchievementInfoWithTaskID(int taskID)
    {
        Dictionary<int, TaskItem> taskItemsES3 = ES3.Load<Dictionary<int, TaskItem>>("Task", "Achievement/Task", new Dictionary<int, TaskItem>());
        foreach (KeyValuePair<int, TaskItem> task in taskItemsES3)
        {
            if (task.Key == taskID)
            {
                int achievementID = task.Value.achievementID;
                Dictionary<int, AchievementItem> achievementItemsES3 = ES3.Load<Dictionary<int, AchievementItem>>("Achievement", "Achievement/Achievement", new Dictionary<int, AchievementItem>());
                foreach (KeyValuePair<int, AchievementItem> achievement in achievementItemsES3)
                {
                    if (achievement.Key == achievementID)
                    {
                        int[] achievementInfo = new int[3];
                        achievementInfo[0] = achievement.Value.achievementID;
                        achievementInfo[1] = achievement.Value.coinReward;
                        achievementInfo[2] = achievement.Value.levelFactorPointReward;
                        return achievementInfo;
                    }
                }
                break;
            }
        }

        return null;
    }

    /// <summary>
    /// Get coinReward, levelFactorPointReward by TaskID
    /// </summary>
    /// <param name="taskID">TaskID</param>
    /// <returns>int[] taskInfo: [coinReward, levelFactorPointReward]</returns>
    public static int[] getTaskInfoWithTaskID(int taskID)
    {
        Dictionary<int, TaskItem> taskItemsES3 = ES3.Load<Dictionary<int, TaskItem>>("Task", "Achievement/Task", new Dictionary<int, TaskItem>());
        foreach (KeyValuePair<int, TaskItem> task in taskItemsES3)
        {
            if (task.Key == taskID)
            {
                int[] taskInfo = new int[2];
                taskInfo[0] = task.Value.coinReward;
                taskInfo[1] = task.Value.levelFactorPointReward;
                return taskInfo;
            }
        }
        return null;
    }
}
