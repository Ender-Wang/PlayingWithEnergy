using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TaskListController : MonoBehaviour
{
    public Transform taskListContent;
    public GameObject taskDescriptionPrefab;
    public TextMeshProUGUI taskNameText;

    public Dictionary<Task, GameObject> taskDict { get; set; }

    public void SetTaskList(Task task)
    {
        gameObject.name = gameObject.name.Replace("(Clone)", "");
        taskNameText.text = task.taskName;
        taskDict = new Dictionary<Task, GameObject>();
        for (int i = 0; i < task.subTasks.Count; i++)
        {
            GameObject taskDescription = Instantiate(taskDescriptionPrefab, taskListContent);
            taskDescription.GetComponent<SubTaskController>().SetSubTask(task.subTasks[i]);
            taskDict.Add(task.subTasks[i], taskDescription);
        }
        ActiveTask(task.subTasks[0]);
    }

    /// <summary>
    /// API: activate the task
    /// </summary>
    /// <param name="task"></param>
    public void ActiveTask(Task task)
    {
        taskDict[task].SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(taskListContent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetChild(0).GetChild(0).GetComponent<RectTransform>());
    }

    /// <summary>
    /// API: Activate the subtask
    /// </summary>
    /// <param name="subTask"></param>
    public void ActiveSubTask(Task subTask)
    {
        taskDict[subTask.parentTask].GetComponent<SubTaskController>().subTaskDict[subTask].SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(taskDict[subTask.parentTask].GetComponent<SubTaskController>().subTaskListContent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(taskDict[subTask.parentTask].GetComponent<SubTaskController>().subTaskListContent.parent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(taskDict[subTask.parentTask].GetComponent<SubTaskController>().subTaskListContent.parent.parent.GetComponent<RectTransform>());
        ActiveTask(subTask.parentTask);
    }

    /// <summary>
    /// API: update the task list when a subtask is finished
    /// </summary>
    /// <param name="subTask"></param>
    public void UpdateTask(Task subTask)
    {
        if (taskDict != null)
        {
            foreach (var task in taskDict.Keys)
            {
                taskDict[task].GetComponent<SubTaskController>().FinishSubTask(subTask);
            }
        }
    }

    /// <summary>
    /// if all subtasks are finished, finish the task
    /// </summary>
    /// <param name="Task"></param>
    public void FinishTask(Task Task)
    {
        if (taskDict.ContainsKey(Task))
        {
            taskDict[Task].GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
            taskDict[Task].transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.green;
        }
    }
}
