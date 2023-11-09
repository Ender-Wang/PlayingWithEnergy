using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UltimateClean
{
    public class AchievementItemUI : MonoBehaviour
    {
        [Header("Achievement Basic Info")]
        public TextMeshProUGUI itemAchievementID;
        public Image itemIcon;
        public TextMeshProUGUI itemTitle;
        public TextMeshProUGUI itemDescription;
        public TextMeshProUGUI itemActiveTaskAmount;

        [Header("Achievement Reward Info")]
        public TextMeshProUGUI itemCoinReward;
        public TextMeshProUGUI itemLevelFactorPointReward;

        public void SetAchievementItemUI(int id, Sprite icon, string title, string description, int progress, int activeTaskAmount, int taskCount, int coinReward, int levelFactorPointReward)
        {
            itemAchievementID.text = id.ToString();
            if (icon != null)
                itemIcon.sprite = icon;
            itemTitle.text = title;
            itemDescription.text = description;
            itemActiveTaskAmount.text = activeTaskAmount + " / " + taskCount;
            itemCoinReward.text = "+" + coinReward.ToString("N0");
            itemLevelFactorPointReward.text = "+" + levelFactorPointReward.ToString("N0");
        }
    }
}