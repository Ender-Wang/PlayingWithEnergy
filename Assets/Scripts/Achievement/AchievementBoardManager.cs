using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Collections;

public class AchievementBoardManager : MonoBehaviour
{
    public TextAsset achievementItemFile;
    public List<AchievementItem> achievementItems { get; set; }
    public GameObject currentAchievementBoardPopup { get; set; }

    public static AchievementBoardManager Instance;

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

    void Start()
    {
        achievementItems = new List<AchievementItem>();
        SetListAchievementItem(achievementItemFile.text);
    }

    private void SetListAchievementItem(string fileContent)
    {
        Stream stream = GenerateStreamFromString(fileContent);
        using (var reader = new StreamReader(stream))
        {
            // Read the first line to get the column headers
            var headers = reader.ReadLine()?.Split('|');

            // Find the indices of the Name, Description, Icon, Reward, RewardAmount, RewardType, Condition, ConditionAmount, ConditionType, IsCompleted, IsClaimed columns
            var idIndex = Array.IndexOf(headers, "AchievementID");
            var statusIndex = Array.IndexOf(headers, "Status");
            var titleIndex = Array.IndexOf(headers, "Title");
            var descriptionIndex = Array.IndexOf(headers, "Description");
            var progressIndex = Array.IndexOf(headers, "Progress");
            var pendingTaskAmountIndex = Array.IndexOf(headers, "PendingTaskAmount");
            var coinRewardIndex = Array.IndexOf(headers, "CoinReward");
            var levelFactorPointRewardIndex = Array.IndexOf(headers, "LevelFactorPointReward");

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var valuesArray = MyCsvParser.parse(line);

                var id = int.Parse(valuesArray[idIndex]);
                var status = bool.Parse(valuesArray[statusIndex]);
                var title = valuesArray[titleIndex];
                var description = valuesArray[descriptionIndex];
                var progress = int.Parse(valuesArray[progressIndex]);
                var pendingTaskAmount = int.Parse(valuesArray[pendingTaskAmountIndex]);
                var coinReward = int.Parse(valuesArray[coinRewardIndex]);
                var levelFactorPointReward = int.Parse(valuesArray[levelFactorPointRewardIndex]);

                AchievementItem achievementItem = new AchievementItem(id, status, null, title, description, progress, pendingTaskAmount, coinReward, levelFactorPointReward);
                achievementItems.Add(achievementItem);
            }
        }
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