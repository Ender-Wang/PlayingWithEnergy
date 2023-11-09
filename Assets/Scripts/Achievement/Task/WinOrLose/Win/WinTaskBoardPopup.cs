using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UltimateClean
{
    public class WinTaskBoardPopup : MonoBehaviour
    {
        public TextMeshProUGUI coinRewardText;
        public TextMeshProUGUI levelFactorPointRewardText;

        /// <summary>
        /// Set the reward coin and level factor point text
        /// </summary>
        /// <param name="coinRewardAmount">The coin reward got from the task</param>
        /// <param name="levelFactorPointRewardAmount">The level factor reward got from the task</param>
        public void SetWinTaskPopup(int coinRewardAmount, int levelFactorPointRewardAmount)
        {
            coinRewardText.text = "Coin +" + coinRewardAmount.ToString("N0");
            levelFactorPointRewardText.text = "Level Factor +" + levelFactorPointRewardAmount.ToString("N0");
        }
    }
}