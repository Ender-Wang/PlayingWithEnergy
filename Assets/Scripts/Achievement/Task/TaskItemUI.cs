using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UltimateClean
{
    public class TaskItemUI : MonoBehaviour
    {
        [Header("Task Basic Info")]
        public TextMeshProUGUI achievementID; // achievement id indicates which achievement this task belongs to
        public TextMeshProUGUI taskId;
        public TextMeshProUGUI status;
        public TextMeshProUGUI title;
        public TextMeshProUGUI description;

        [Header("Start Task Button")]
        public Button start;
        public Button completed;

        [Header("Task Detail")]
        public TextMeshProUGUI taskLocation;
        public TextMeshProUGUI timeLimit;
        public TextMeshProUGUI difficulty;
        public TextMeshProUGUI coinReward;
        public TextMeshProUGUI levelFactorPointReward;

        public void SetTaskItemUI(int achievementID, int taskID, bool status, string title, string description, Vector3[] taskLocation, int timeLimit, string difficulty, int coinReward, int levelFactorPointReward)
        {
            this.achievementID.text = achievementID.ToString();
            this.taskId.text = taskID.ToString();
            this.status.text = status ? "Completed" : "Pending";
            this.title.text = title;
            this.description.text = description;

            start.gameObject.SetActive(!status);
            completed.gameObject.SetActive(status);

            //Show the first possible task location only
            this.taskLocation.text = taskLocation[0].ToString();
            this.timeLimit.text = timeLimit + " mins";
            this.difficulty.text = difficulty;
            this.coinReward.text = "+" + coinReward.ToString("N0");
            this.levelFactorPointReward.text = "+" + levelFactorPointReward.ToString("N0");
        }
    }
}
