using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Collections;
using UltimateClean;

public class TaskManager : MonoBehaviour
{
    public TextAsset taskItemFile;
    public List<TaskItem> taskItems { get; set; }
    public GameObject currentTaskPopup { get; set; }
    public static TaskManager Instance;

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

    // Start is called before the first frame update
    void Start()
    {
        taskItems = new List<TaskItem>();
        SetListTaskItem(taskItemFile.text);
    }

    private void SetListTaskItem(string fileContent)
    {
        Stream stream = GenerateStreamFromString(fileContent);
        using (var reader = new StreamReader(stream))
        {
            // Read the first line to get the column headers
            var headers = reader.ReadLine()?.Split('|');

            // Find the indices of the Name, Description, Icon, Reward, RewardAmount, RewardType, Condition, ConditionAmount, ConditionType, IsCompleted, IsClaimed columns
            var achievementIDIndex = Array.IndexOf(headers, "AchievementID");
            var taskIDIndex = Array.IndexOf(headers, "TaskID");
            var statusIndex = Array.IndexOf(headers, "Status");
            var titleIndex = Array.IndexOf(headers, "Title");
            var descriptionIndex = Array.IndexOf(headers, "Description");
            var taskLocationIndex = Array.IndexOf(headers, "TaskLocation");
            var timeLimitIndex = Array.IndexOf(headers, "TimeLimit");
            var difficultyIndex = Array.IndexOf(headers, "Difficulty");
            var coinRewardIndex = Array.IndexOf(headers, "CoinReward");
            var LevelFactorPointRewardIndex = Array.IndexOf(headers, "LevelFactorPointReward");

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var valuesArray = MyCsvParser.parse(line);

                var achievementID = int.Parse(valuesArray[achievementIDIndex]);
                var taskID = int.Parse(valuesArray[taskIDIndex]);
                var status = bool.Parse(valuesArray[statusIndex]);
                var title = valuesArray[titleIndex];
                var description = valuesArray[descriptionIndex];
                var taskLocation = taskLocationParser(valuesArray[taskLocationIndex]);
                var timeLimit = int.Parse(valuesArray[timeLimitIndex]);
                var difficulty = valuesArray[difficultyIndex];
                var coinReward = int.Parse(valuesArray[coinRewardIndex]);
                var LevelFactorPointReward = int.Parse(valuesArray[LevelFactorPointRewardIndex]);

                TaskItem taskItem = new TaskItem(achievementID, taskID, status, title, description, taskLocation, timeLimit, difficulty, coinReward, LevelFactorPointReward);
                taskItems.Add(taskItem);
            }
        }
    }

    Vector3[] taskLocationParser(string location)
    {
        string[] locationsArray = location.Split(';');
        Vector3[] locationV3 = new Vector3[locationsArray.Length];
        for (int i = 0; i < locationsArray.Length; i++)
        {
            string[] locationArray = locationsArray[i].Replace("(", "").Replace(")", "").Split(',');
            Vector3 v3 = new Vector3(float.Parse(locationArray[0]), float.Parse(locationArray[1]), float.Parse(locationArray[2]));
            locationV3[i] = v3;
        }
        return locationV3;
    }

    /// <summary>
    /// Set and show the win task popup board
    /// </summary>
    /// <param name="coinRewardAmount">Coin reward for winning the task</param>
    /// <param name="levelFactorPointRewardAmount">Level Factor reward for winning the task</param>
    public void WinTask(int coinRewardAmount, int levelFactorPointRewardAmount)
    {
        GetComponent<WinTaskBoardPopupOpener>().SetRewardAmount(coinRewardAmount, levelFactorPointRewardAmount);
    }

    /// <summary>
    /// Set and show the lose task popup board
    /// </summary>
    /// <param name="loseReason">The reason why the task failed</param>
    public void LoseTask(string loseReason)
    {
        GetComponent<LoseTaskBoardPopupOpener>().SetLoseReason(loseReason);
    }
    private Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}
