using UnityEngine;
using System;

[ES3Serializable, Serializable]
public class AchievementItem
{

    [ES3Serializable]
    public int achievementID { get; set; }
    [ES3Serializable]
    public bool status { get; set; } // true = completed, false = not completed
    [ES3Serializable]
    public Sprite icon { get; set; }
    [ES3Serializable]
    public string title { get; set; }
    [ES3Serializable]
    public string description { get; set; }
    [ES3Serializable]
    public int progress { get; set; }
    [ES3Serializable]
    public int pendingTaskAmount { get; set; }
    [ES3Serializable]
    public int coinReward { get; set; }
    [ES3Serializable]
    public int levelFactorPointReward { get; set; }

    public AchievementItem()
    {
        achievementID = 0;
        status = false;
        icon = null;
        title = "Default";
        description = "Default";
        progress = 0;
        pendingTaskAmount = 0;
        coinReward = 0;
        levelFactorPointReward = 0;
    }

    public AchievementItem(int achievementID, bool status, Sprite icon, string title, string description, int progress, int pendingTaskAmount, int coinReward, int levelFactorPointReward)
    {
        this.achievementID = achievementID;
        this.status = status;
        this.icon = icon;
        this.title = title;
        this.description = description;
        this.progress = progress;
        this.pendingTaskAmount = pendingTaskAmount;
        this.coinReward = coinReward;
        this.levelFactorPointReward = levelFactorPointReward;
    }
}