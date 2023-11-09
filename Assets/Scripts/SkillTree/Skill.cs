using System.Collections.Generic;
using System;
using UnityEngine;

[ES3Serializable]
public class Skill
{
    [ES3Serializable]
    private string name;
    [ES3Serializable]
    private float developTime;
    [ES3Serializable]
    private int requiredLevel;
    [ES3Serializable]
    private DateTime startTime;
    [ES3Serializable]
    private List<Skill> prerequisites;
    [ES3Serializable]
    private int cost;
    [ES3Serializable]
    private int maxlevel = 1;
    [ES3Serializable]
    private int currentLevel = 0;
    [ES3Serializable]
    private bool onLearning = false;
    [ES3Serializable]
    public string description {get; set;}
    [ES3Serializable]
    // the image name
    public string iconPath {get; set;}

    public Skill (string name, float developTime, int requiredLevel, int cost, int maxlevel, DateTime startTime, List<Skill> prerequisites) {
        this.name = name;
        this.developTime = developTime;
        this.requiredLevel = requiredLevel;
        this.startTime = startTime;
        this.cost = cost;
        this.prerequisites = prerequisites;
        this.maxlevel = maxlevel;
    }

    public Skill (string name, float developTime, int requiredLevel, int cost, int maxlevel, List<Skill> prerequisites) {
        this.name = name;
        this.developTime = developTime;
        this.requiredLevel = requiredLevel;
        this.cost = cost;
        this.prerequisites = prerequisites;
        this.maxlevel = maxlevel;
    }

    public Skill (string name, float developTime, int requiredLevel, int cost, int maxlevel) {
        this.name = name;
        this.developTime = developTime;
        this.requiredLevel = requiredLevel;
        this.cost = cost;
        this.maxlevel = maxlevel;
        this.prerequisites = new List<Skill>();
    }

    public Skill(){}

    public void addPrerequisite(Skill skill) {
        if (!this.prerequisites.Contains(skill))
            this.prerequisites.Add(skill);
    }

    public List<Skill> getPrerequisites() {
        return this.prerequisites;
    }

    /// <summary>
    /// start develop this skill
    /// </summary>
    /// <returns>status code: 200 => successful, 400 => fail </returns>
    public Dictionary<int, string> StartDevelop(){
        // TODO: money withdraw, increase cost and develop time
        Dictionary<int, string> res = new Dictionary<int, string>();
        int statusCode = 200;
        string message = SkillManager.Instance.getNotification("success");
        if (this.currentLevel == this.maxlevel) {
            statusCode = 400;
            message = SkillManager.Instance.getNotification("maxlevel");
            res.Add(statusCode, message);
            return res;
        }
        if (GameManager.Instance.level < this.requiredLevel){
            statusCode = 400;
            message = string.Format(SkillManager.Instance.getNotification("level"), GameManager.Instance.level, this.requiredLevel);
            res.Add(statusCode, message);
            return res;
        }
        if (GameManager.Instance.energy < this.cost){
            statusCode = 400;
            message = SkillManager.Instance.getNotification("energy");
            res.Add(statusCode, message);
            return res;
        }
        foreach (Skill skill in prerequisites){
            if (!skill.isAvailable()) {
                statusCode = 400;
                message = string.Format(SkillManager.Instance.getNotification("prerequisite"), skill.name);
                res.Add(statusCode, message);
                return res;
            } 
        }
        res.Add(statusCode, message);
        this.startTime = DateTime.Now;
        this.onLearning = true;
        return res;
    }

    public bool isAvailable(){
        return this.currentLevel > 0;
    }

    public float getDevelopTime(){
        return this.developTime;
    }

    public string getName(){
        return this.name;
    }

    public int getCost(){
        return this.cost;
    }

    public int getRequiredLevel() {
        return this.requiredLevel;
    }

    public float TimeNeedToFinish() {
        float leftTime = developTime - (DateTime.Now - startTime).Seconds;
        return leftTime >= 0 ? leftTime : 0;
    }

    public Skill Clone(){
        return (Skill)this.MemberwiseClone();
    }

    public bool isOnLearning(){
        return this.onLearning;
    }

    public void LearningEnd(){
        this.onLearning = false;
    }

    public DateTime getStartTime() {
        return this.startTime;
    }

    public int getMaxLevel(){
        return this.maxlevel;
    }
    public int getCurrentLevel(){
        return this.currentLevel;
    }

    public void resetStartTime(){
        this.startTime = DateTime.MinValue;
    }

    public void upgrade(){
        this.currentLevel+=1;
    }
}