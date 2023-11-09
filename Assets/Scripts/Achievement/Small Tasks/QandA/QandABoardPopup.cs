using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateClean
{
    public class QandABoardPopup : MonoBehaviour
    {
        public GameObject QandAItemPrefab;
        public Transform QandAItemParent;

        public GameObject QandAITaskTitle;

        private GameObject m_QandAItem;

        void OnEnable()
        {
            QandAITaskTitle.GetComponent<TMPro.TextMeshProUGUI>().text = LoadQandATaskTitleFromESC();
        }
        public void SetQandAPopup()
        {
            foreach (QandAItem QandAItem in QandAManager.Instance.QandAItems)
            {
                //show the QandA items that have specific id => match to the parent Task id
                if (QandAItem.id == 1)
                {
                    m_QandAItem = Instantiate(QandAItemPrefab, QandAItemParent);
                    m_QandAItem.GetComponent<QandAItemUI>().SetQandAItemUI(QandAItem.id.ToString(), QandAItem.question, QandAItem.optionA, QandAItem.optionB, QandAItem.solution, QandAItem.point.ToString());
                }
            }
        }

        string LoadQandATaskTitleFromESC()
        {
            Dictionary<int, TaskItem> taskItemsES3 = ES3.Load<Dictionary<int, TaskItem>>("Task", "Achievement/Task", new Dictionary<int, TaskItem>());
            foreach (KeyValuePair<int, TaskItem> task in taskItemsES3)
            {
                if (task.Key == 4)
                {
                    return task.Value.title;
                }
            }
            return "";
        }
    }
}
