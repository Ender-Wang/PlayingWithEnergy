using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AchievementButtonText : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI hoverText;
    public TextAsset taskCSV;
    private int activeTaskNumber;
    public static AchievementButtonText Instance;
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

    void OnEnable()
    {
        AchievementButtonTextNumber();
    }

    public void AchievementButtonTextNumber()
    {
        activeTaskNumber = CountActiveTaskNumber();
        buttonText.text = activeTaskNumber.ToString();
        hoverText.text = activeTaskNumber.ToString() + " active tasks to do...";
    }

    //Count the number of non-completed
    public int CountActiveTaskNumber()
    {
        int activeTaskNumber = 0;
        //Count from ES3 if ES3 exist, else count from CSV
        Dictionary<int, TaskItem> taskItemsES3 = ES3.Load<Dictionary<int, TaskItem>>("Task", "Achievement/Task", new Dictionary<int, TaskItem>());
        if (taskItemsES3.Count != 0)
        {
            foreach (KeyValuePair<int, TaskItem> task in taskItemsES3)
            {
                if (task.Value.status == false)
                {
                    activeTaskNumber++;
                }
            }
            return activeTaskNumber;
        }
        else
        {
            return CountActiveTaskNumberInCSV();
        }
    }

    int CountActiveTaskNumberInCSV()
    {
        int count = 0;
        string[] lines = taskCSV.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split('|');
            if (values[2] == "false")
            {
                count++;
            }
        }
        return count;
    }
}
