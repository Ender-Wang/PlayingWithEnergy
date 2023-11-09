using UnityEngine;
using System.Collections.Generic;

public class TaskCompletionManager : MonoBehaviour
{
    public static TaskCompletionManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// [Task Failed] Show task lose popup on task failure.
    /// </summary>
    /// <param name="loseReason">Reason of task failed</param>
    public void taskFailed(string loseReason)
    {
        TaskManager.Instance.LoseTask(loseReason);
    }

    /// <summary>
    /// [Task Completed] Show task win popup on task completion. Update task status in ES3, collect all rewards of the task, check relevant achievement completion status.
    /// </summary>
    /// <param name="taskID">TaskID</param>
    public void taskCompleted(int taskID)
    {
        // Obtain task information with taskID
        int[] taskInfo = AchievementIOManager.getTaskInfoWithTaskID(taskID);
        int coinAmount = taskInfo[0];
        int levelFactorAmount = taskInfo[1];

        //Set the win task popup
        TaskManager.Instance.WinTask(coinAmount, levelFactorAmount);


        TimeLimitationManager.Instance.StopTaskTimer();

        //Update Task status in ES3 => update task status shown in task board
        AchievementIOManager.updateTaskRegister(taskID);

        //Collect all rewards of the task: coin and level factor
        GameManager.Instance.AddMoney(coinAmount);
        //Currently all tasks are the type of Energy Provision
        // LevelManager.getLevelFactor("Energy Provision").update(levelFactorAmount);
        GameManager.Instance.score += levelFactorAmount;


        //Obtain achievement information with taskID, then check achievement completion status
        int[] achievementInfo = AchievementIOManager.getAchievementInfoWithTaskID(taskID);
        int achievementID = achievementInfo[0];
        int achievementCoinReward = achievementInfo[1];
        int achievementLevelFactorReward = achievementInfo[2];
        checkAchievementAchievedStatus(achievementID, achievementCoinReward, achievementLevelFactorReward);

        //Update achievementButtonText
        AchievementButtonText.Instance.AchievementButtonTextNumber();
    }

    /// <summary>
    /// Check relevant achievement completion status on task completion, if the achievement is achieved, collect the achievement reward.
    /// </summary>
    void checkAchievementAchievedStatus(int achievementID, int achievementCoinReward, int achievementLevelFactorReward)
    {
        if (checkAchievementAchieved(achievementID))
        {
            //Collect all rewards of the achievement
            GameManager.Instance.AddMoney(achievementCoinReward);
            // LevelManager.getLevelFactor("Energy Provision").update(achievementLevelFactorReward);
            GameManager.Instance.score += achievementLevelFactorReward;

            //Update Achievement Register -> update completed achievement record in ES3
            AchievementIOManager.updateAchievementRegister(achievementID);
        }
    }
    /// <summary>
    /// Check whether the achievement is achieved. If yes, set the achievement coin reward and level factor reward, then return true. If not, return false.
    /// </summary>
    /// <returns></returns>

    bool checkAchievementAchieved(int achievementID)
    {
        //Check corresponding task status in task ES3
        Dictionary<int, TaskItem> taskItemsES3 = ES3.Load<Dictionary<int, TaskItem>>("Task", "Achievement/Task", new Dictionary<int, TaskItem>());
        foreach (KeyValuePair<int, TaskItem> taskItemES3 in taskItemsES3)
        {
            if (taskItemES3.Value.achievementID == achievementID)
            {
                Debug.Log("Task ID and achievement ID checked: " + achievementID + " " + taskItemES3.Value.achievementID);
                if (taskItemES3.Value.status == false)
                {
                    return false;
                }
            }
        }
        return true;
    }
}