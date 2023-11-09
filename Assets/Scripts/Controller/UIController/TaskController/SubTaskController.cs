using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SubTaskController : MonoBehaviour
{
    public Transform subTaskListContent;
    public GameObject subTaskPrefab;
    public TextMeshProUGUI subTaskNameText;
    public Dictionary<Task, GameObject> subTaskDict { get; set; }

    public void SetSubTask(Task subTask)
    {
        subTaskDict = new Dictionary<Task, GameObject>();
        string[] names = subTask.taskName.Split('-');
        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(TaskOnClick);
        subTaskNameText.text = names[names.Length - 1];
        for (int i = 0; i < subTask.subTasks.Count; i++)
        {
            GameObject subTaskDescription = Instantiate(subTaskPrefab, subTaskListContent);
            names = subTask.subTasks[i].taskName.Split('-');
            subTaskDescription.GetComponentInChildren<TextMeshProUGUI>().text = names[names.Length - 1];
            subTaskDescription.GetComponentInChildren<Button>().onClick.AddListener(SubTaskOnClick);
            subTaskDict.Add(subTask.subTasks[i], subTaskDescription);
            subTaskDescription.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    void SubTaskOnClick()
    {
        //TODO: the subtask action
    }

    void TaskOnClick()
    {
        //TODO: the task action
    }

    public void FinishSubTask(Task subTask)
    {
        if (subTaskDict.ContainsKey(subTask))
        {
            subTaskDict[subTask].GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
            subTaskDict[subTask].GetComponentInChildren<Button>().interactable = false;
            subTaskDict[subTask].GetComponentInChildren<Image>().color = Color.green;
            // subTaskDict.Remove(subTask);
        }
    }
}
