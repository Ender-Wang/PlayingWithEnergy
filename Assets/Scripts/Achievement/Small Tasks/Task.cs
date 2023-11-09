using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TaskType
{
    FindNPC,
    FindObjects,
    Other,
}
public class Task : MonoBehaviour
{
    public int taskID = 0;
    public string taskName = "Task";
    public TaskType taskType = TaskType.FindNPC;

    // nullable
    public Task parentTask;
    public List<Task> subTasks = new List<Task>();

    // ---- Information which is needed when finishing task ----
    public int coinRewardAmount = 0;
    public int levelFactorPointRewardAmount = 0;
    public bool isFinished = false;
    public Vector3 playerPositionRecord;
    public int index = 0;

    GameObject taskList;

    void Start()
    {
        taskList = GameObject.Find("TaskList");
    }

    public void AddSubTask(Task subTask)
    {
        subTasks.Add(subTask);
        subTask.parentTask = this;
        subTask.taskID = taskID;
    }

    public void SelfsetNextActive()
    {
        int i = index;
        foreach (Task t in subTasks)
        {
            if (i == 0)
            {
                t.gameObject.SetActive(true);
                taskList.GetComponent<TaskListController>()?.ActiveSubTask(t);
                break;
            }
            --i;
        }
    }

    public bool ParentCheckSubTask()
    {
        bool allSubFinished = true;
        foreach (Task st in parentTask.subTasks)
        {
            if (!st.isFinished)
            {
                allSubFinished = false;
                break;
            }
        }
        return allSubFinished;
    }

    bool SelfCheckSubTask()
    {
        bool allSubFinished = true;
        foreach (Task st in this.subTasks)
        {
            if (!st.isFinished)
            {
                allSubFinished = false;
                break;
            }
        }
        return allSubFinished;
    }

    // only set deactive to true if task is finish
    public void FinishTask(bool deactive = false)
    {
        // parentTask.setNextActive();
        if (!SelfCheckSubTask())
        {
            SelfsetNextActive();
            return;
        }

        // StartCoroutine(Deactive(deactive));

        isFinished = deactive;

        taskList.GetComponent<TaskListController>()?.UpdateTask(this);
        taskList.GetComponent<TaskListController>()?.FinishTask(this); // Finish the task list
        // this.gameObject.SetActive(TaskType.FindNPC == this.taskType); // hide the subtask object if it is a FindObj task
        if (parentTask.parentTask)
        {
            parentTask.GetComponent<CharacterDialogSystem>()?.NextState(); // next state of dialog
        }

        if (deactive)
        {
            parentTask.index++;
            if (gameObject.GetComponent<Obj>())
                Destroy(gameObject);
            else
                Destroy(this.gameObject, 3f);
        }

        if (parentTask.parentTask == null)
        {
            if (ParentCheckSubTask())
            {
                Destroy(parentTask.gameObject, 3f);
                TaskCompletionManager.Instance.taskCompleted(taskID);
                TimeLimitationManager.Instance.ActivateTimerBoard(false);
                GameObject.FindWithTag("Player").transform.position = playerPositionRecord;
                Destroy(taskList);
            }
            else
            {
                parentTask.gameObject.transform.GetChild(parentTask.index).gameObject.SetActive(true);
                taskList.GetComponent<TaskListController>()?.ActiveTask(parentTask.gameObject.transform.GetChild(parentTask.index).GetComponent<Task>());
            }
        }
    }

    IEnumerator Deactive(bool deactive)
    {
        yield return new WaitForSeconds(3);
        this.gameObject.SetActive(!deactive);
    }
}