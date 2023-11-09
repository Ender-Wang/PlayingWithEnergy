using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UltimateClean
{

    public class AchievementBoardPopup : MonoBehaviour
    {
        public GameObject achievementItemPrefab;
        public Transform achievementItemParent;
        public GameObject taskItemPrefab;
        // public Transform taskItemParent;
        private GameObject m_achievementItem;
        private GameObject m_taskItem;

        /// <summary>
        /// Set up the achievement board popup, which includes the achievement items and task items
        /// </summary>
        public void SetAchievementBoardPopup()
        {
            //load achievementItems and taskItems from ES3
            Dictionary<int, AchievementItem> achievementItemsES3 = ES3.Load<Dictionary<int, AchievementItem>>("Achievement", "Achievement/Achievement", new Dictionary<int, AchievementItem>());
            Dictionary<int, TaskItem> taskItemsES3 = ES3.Load<Dictionary<int, TaskItem>>("Task", "Achievement/Task", new Dictionary<int, TaskItem>());

            //if there is no achievementItems/taskItems in ES3, then instantiate AchievementItemUI and TaskItemUI, save them to ES3
            if (achievementItemsES3.Count == 0)
            {
                foreach (AchievementItem achievementItem in AchievementBoardManager.Instance.achievementItems)
                {
                    int taskCount = 0;
                    int activeTaskCount = 0;
                    m_achievementItem = Instantiate(achievementItemPrefab, achievementItemParent, false);
                    foreach (TaskItem taskItem in TaskManager.Instance.taskItems)
                    {
                        if (taskItem.achievementID == achievementItem.achievementID)
                        {
                            taskCount++;
                            m_taskItem = Instantiate(taskItemPrefab, achievementItemParent, false);
                            m_taskItem.GetComponent<TaskItemUI>().SetTaskItemUI(taskItem.achievementID, taskItem.taskID, taskItem.status, taskItem.title, taskItem.description, taskItem.taskLocation, taskItem.timeLimit, taskItem.difficulty, taskItem.coinReward, taskItem.levelFactorPointReward);
                        }
                        if (taskItem.achievementID == achievementItem.achievementID && taskItem.status == false)
                        {
                            activeTaskCount++;
                        }
                    }
                    // set pendingTaskAmount to activeTaskCount
                    m_achievementItem.GetComponent<AchievementItemUI>().SetAchievementItemUI(achievementItem.achievementID, achievementItem.icon, achievementItem.title, achievementItem.description, achievementItem.progress, activeTaskCount, taskCount, achievementItem.coinReward, achievementItem.levelFactorPointReward);
                }
                AchievementIOManager.LoadAchievementRegister();
                AchievementIOManager.LoadTaskRegister();
            }
            else
            {
                //if there is achievementItems/taskItems in ES3, then load them from ES3 and set respective UI
                foreach (KeyValuePair<int, AchievementItem> achievementItemES3 in achievementItemsES3)
                {
                    int taskCount = 0;
                    int activeTaskCount = 0;
                    m_achievementItem = Instantiate(achievementItemPrefab, achievementItemParent, false);
                    foreach (KeyValuePair<int, TaskItem> taskItemES3 in taskItemsES3)
                    {
                        if (taskItemES3.Value.achievementID == achievementItemES3.Value.achievementID)
                        {
                            taskCount++;
                            m_taskItem = Instantiate(taskItemPrefab, achievementItemParent, false);
                            m_taskItem.GetComponent<TaskItemUI>().SetTaskItemUI(taskItemES3.Value.achievementID, taskItemES3.Value.taskID, taskItemES3.Value.status, taskItemES3.Value.title, taskItemES3.Value.description, taskItemES3.Value.taskLocation, taskItemES3.Value.timeLimit, taskItemES3.Value.difficulty, taskItemES3.Value.coinReward, taskItemES3.Value.levelFactorPointReward);
                        }
                        if (taskItemES3.Value.achievementID == achievementItemES3.Value.achievementID && taskItemES3.Value.status == false)
                        {
                            activeTaskCount++;
                        }
                    }
                    // set pendingTaskAmount to activeTaskCount
                    m_achievementItem.GetComponent<AchievementItemUI>().SetAchievementItemUI(achievementItemES3.Value.achievementID, achievementItemES3.Value.icon, achievementItemES3.Value.title, achievementItemES3.Value.description, achievementItemES3.Value.progress, activeTaskCount, taskCount, achievementItemES3.Value.coinReward, achievementItemES3.Value.levelFactorPointReward);
                }
            }
        }
    }
}