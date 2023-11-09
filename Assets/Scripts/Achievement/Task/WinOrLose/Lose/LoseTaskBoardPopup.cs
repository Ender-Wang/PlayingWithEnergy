using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace UltimateClean
{
    public class LoseTaskBoardPopup : MonoBehaviour
    {
        public TextMeshProUGUI loseReasonText;
        public void SetLoseTaskPopup(string loseReason)
        {
            loseReasonText.text = loseReason;
        }
    }
}