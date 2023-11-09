using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;

public class ToggleTaskList : MonoBehaviour
{
    public TextMeshProUGUI achievementID;
    /// <summary>
    /// Toggle in/active state of all tasks with the same id with the achievement.
    /// </summary>
    public void openTaskList()
    {
        //find all Task objects regardless of its active state
        List<GameObject> allTasks = new List<GameObject>();
        Transform[] canvasRootObjects = GameManager.Instance.canvas.transform.GetComponentsInChildren<Transform>(true);

        foreach (Transform go in canvasRootObjects)
        {
            if (go.gameObject.name.Contains("Achievement"))
            {
                Transform[] achievementObjects = go.transform.GetComponentsInChildren<Transform>(true);
                foreach (Transform task in achievementObjects)
                {
                    if (task.gameObject.name.Contains("Task"))
                    {
                        allTasks.Add(task.gameObject);
                    }
                }
            }
        }

        //set active state of all tasks with the same id with the achievement
        foreach (GameObject task in allTasks)
        {
            TextMeshProUGUI[] allID = task.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI id in allID)
            {
                if (id.name == "Achievement ID")
                {
                    if (id.text.ToString() == achievementID.text.ToString())
                    {
                        bool isActive = task.activeSelf;
                        task.SetActive(!isActive);
                    }
                }
            }
        }
    }
}
