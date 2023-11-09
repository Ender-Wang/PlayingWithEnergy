using UnityEngine;
using TMPro;

namespace UltimateClean
{
    public class QandAItemUI : MonoBehaviour
    {
        public TextMeshProUGUI id;
        public TextMeshProUGUI question;
        public TextMeshProUGUI optionA;
        public TextMeshProUGUI optionB;
        public TextMeshProUGUI solution;
        public TextMeshProUGUI point;   //point of the question

        public void SetQandAItemUI(string id, string question, string optionA, string optionB, string solution, string point)
        {
            this.id.text = id;
            this.question.text = question;
            this.optionA.text = optionA;
            this.optionB.text = optionB;
            this.solution.text = solution;
            this.point.text = point;
        }
    }
}