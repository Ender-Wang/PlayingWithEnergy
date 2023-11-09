using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using TMPro;

public class SkillManager : MonoBehaviour
{   
    public static SkillManager Instance {get; private set;}

    // these are 3 CSV file about user skills
    public TextAsset skillRegisterCSV;
    public TextAsset descriptionCSV;
    public TextAsset notificationCSV;
    
    public GameObject notificationPrefab;
    public GameObject currentSkillTreePopup { get; set; }
    private Dictionary<string, Skill> skillRegister = new Dictionary<string, Skill>();
    private Dictionary<string, Skill> userSkill;
    private Dictionary<string, string> notification = new Dictionary<string, string>();
    private string es3key = "UserSkill";
    private string es3filepath = "Skill/UserSkill";
    void Awake()
    {
        if (Instance == null){
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void OnDestroy() {
        if (Instance == this) {
            ES3.Save<Dictionary<string, Skill>>(es3key, userSkill, es3filepath);
        }
    }
    // Start is called before the first frame update
    void Start()
    {   
        LoadNotification();
        userSkill = ES3.Load<Dictionary<string, Skill>>(es3key, es3filepath, new Dictionary<string, Skill>());
        if (userSkill.Count == 0) {
            PrepareSkillRegister();
            userSkill = skillRegister;
        } else {
            RecoverSkill();
        }
    }

    /// <summary>
    /// loading user skill when game start
    /// </summary>
    void RecoverSkill() {
        foreach(KeyValuePair<string, Skill> kvp in userSkill) {
            if (kvp.Value.isOnLearning()){
                float waitSeconds = kvp.Value.TimeNeedToFinish();
                StartCoroutine(SkillReady(waitSeconds, kvp.Value));
            }
        }
    }

    public void invokeSkillReady(float seconds, Skill skill) {
        StartCoroutine(SkillReady(seconds, skill));
    }

    /// <summary>
    /// function is invoked, when skill finishes developing. This Coroutine is started when the game is quitted before the skill learning progress finishs
    /// </summary>
    /// <param name="seconds">wait seconds</param>
    /// <param name="skill"></param>
    /// <returns>function is invoked, when skill finishes developing.</returns>
    public IEnumerator SkillReady(float seconds, Skill skill) {
        yield return new WaitForSeconds(seconds);
        string message;
        if (skill.getCurrentLevel() == 0)
            message = "You successfully developed " + skill.getName() + ", a shop item is now released!";
        else
            message = "You successfully upgraded " + skill.getName() + "!";

        GameObject obj = Instantiate(notificationPrefab, GameObject.Find("Canvas").transform);
        obj.transform.FindGrandChild("Message").GetComponent<TextMeshProUGUI>().text = message;
        Destroy(obj, 5);

        skill.LearningEnd();
        skill.upgrade();
        skill.resetStartTime();
        ShopManager.Instance.ActivateShopItem(skill.getName(), skill.getCurrentLevel());
    }

    // load user notification after user clicking on learn skill button
    void LoadNotification () {
        // using (var reader = new StreamReader(path))
        using(var reader = new StreamReader(GenerateStreamFromString(notificationCSV.text)))
        {
            // Read the first line to get the column headers
            var headers = reader.ReadLine()?.Split(';');

            // Find the indices of the Name, Category, Description, Price, Contribution and Icon columns
            var nameIndex = Array.IndexOf(headers, "name");
            var notificationIndex = Array.IndexOf(headers, "notification");

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var valuesArray = line.Split(';');
                if (valuesArray == null || valuesArray.Length < 2)
                {
                    continue;
                }
                string name = valuesArray[nameIndex];
                string message = valuesArray[notificationIndex];
                notification.Add(name, message);
            }
        }
    }

    /// <summary>
    /// loading global skill register
    /// </summary>
    /// <param name="csvFile">csv file path</param>
    void PrepareSkillRegister () {
        Stream csvFile = GenerateStreamFromString(skillRegisterCSV.text);
        Stream descriptionFile = GenerateStreamFromString(descriptionCSV.text);
        using (var reader = new StreamReader(csvFile))
        {
            // Read the first line to get the column headers
            var headers = reader.ReadLine()?.Split(';');

            // Find the indices of the Name, Category, Description, Price, Contribution and Icon columns
            var nameIndex = Array.IndexOf(headers, "name");
            var developTimeIndex = Array.IndexOf(headers, "developTime");
            var requiredLevelIndex = Array.IndexOf(headers, "requiredLevel");
            var costIndex = Array.IndexOf(headers, "cost");
            var prerequisitesIndex = Array.IndexOf(headers, "prerequisites");
            var iconIndex = Array.IndexOf(headers, "icon");
            var maxlevelIndex = Array.IndexOf(headers, "maxlevel");

            // convert each record in SkillRegister to Skill
            Dictionary<string, string[]> tempSkillPrerequisites = new Dictionary<string, string[]>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var valuesArray = line.Split(';');
                if (valuesArray == null || valuesArray.Length < 6)
                {
                    continue;
                }
                string name = valuesArray[nameIndex];
                float developTime = float.Parse(valuesArray[developTimeIndex]);
                int requiredLevel = int.Parse(valuesArray[requiredLevelIndex]);
                int cost = int.Parse(valuesArray[costIndex]);
                int maxlevel = int.Parse(valuesArray[maxlevelIndex]);
                string[] prerequisites = valuesArray[prerequisitesIndex].Split('/');
                Skill skill = new Skill(name, developTime, requiredLevel, cost, maxlevel);
                skill.iconPath = valuesArray[iconIndex];
                skillRegister.Add(name, skill);
                if (prerequisites.Length > 0 && !prerequisites[0].Equals("-"))
                    tempSkillPrerequisites.Add(name, prerequisites);
            }

            // find skill dependencies
            foreach(KeyValuePair<string, string[]> kvp in tempSkillPrerequisites) {
                if (kvp.Value.Length != 0) {
                    foreach(string skillName in kvp.Value) {
                        skillRegister[kvp.Key].addPrerequisite(skillRegister[skillName]);
                    }
                }
            }
        }

        // load skill description
        using (var reader = new StreamReader(descriptionFile))
        {
            // Read the first line to get the column headers
            var headers = reader.ReadLine()?.Split(';');

            // Find the indices of the Name, Category, Description, Price, Contribution and Icon columns
            var nameIndex = Array.IndexOf(headers, "name");
            var descriptionIndex = Array.IndexOf(headers, "description");

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var valuesArray = line.Split(';');
                skillRegister[valuesArray[nameIndex]].description = valuesArray[descriptionIndex];
            }
        }
    }

    void Test() {
        foreach(KeyValuePair<string, Skill> kvp in skillRegister){
            string log = kvp.Key + ": ";
            foreach(Skill skill in kvp.Value.getPrerequisites()){
                log += skill.getName();
                log += "/";
            }
            Debug.Log(log);
            Debug.Log("============");
        }
    }

    public Dictionary<string, Skill> getUserSkills(){
        return this.userSkill;
    }

    public string getNotification(string key) {
        if (notification.ContainsKey(key))
            return notification[key];
        else
            return "There is no record with this key. Please contact developer to add one to the file.";
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
