using System;
using UnityEngine;

namespace UltimateClean
{
    public class LevelController : LoadingBarController
    {
        // public static LevelController Instance;

        // protected override void CreateInstance()
        // {
        //     if (Instance == null) {
        //         Instance = this;
        //     }


        //     if (Instance != this)
        //         Destroy(gameObject);

        //     // GameManager.Instance.player = gameObject;
        // }

        protected override float loadingBarCalculation()
        {
            // Debug.Log("Loading bar calculation: " + GameManager.Instance.score + " / " + Settings.totalLevel[GameManager.Instance.level]);
            return (float)Math.Round((decimal)GameManager.Instance.tempScore / Settings.totalLevel[GameManager.Instance.level], 2);
        }

        protected override void ToolTipSetUp()
        {
            toolTip.text = GameManager.Instance.tempScore.ToString() + " / " + Settings.totalLevel[GameManager.Instance.level].ToString() + "\nCurrent XP / XP to next level";
        }
    }
}
