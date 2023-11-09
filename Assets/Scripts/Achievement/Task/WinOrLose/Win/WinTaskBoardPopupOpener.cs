using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UltimateClean
{
    public class WinTaskBoardPopupOpener : PopupOpener
    {
        private int m_coinRewardAmount;
        private int m_levelFactorPointRewardAmount;
        public override void OpenPopup()
        {
            base.OpenPopup();
            m_popup.GetComponent<WinTaskBoardPopup>().SetWinTaskPopup(m_coinRewardAmount, m_levelFactorPointRewardAmount);
        }

        public void SetRewardAmount(int coinRewardAmount, int levelFactorPointRewardAmount)
        {
            m_coinRewardAmount = coinRewardAmount;
            m_levelFactorPointRewardAmount = levelFactorPointRewardAmount;
            OpenPopup();
        }
    }
}