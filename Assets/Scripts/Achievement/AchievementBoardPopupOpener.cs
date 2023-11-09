using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateClean
{
    public class AchievementBoardPopupOpener : PopupOpener
    {
        public override void OpenPopup()
        {
            base.OpenPopup();
            if (AchievementBoardManager.Instance.currentAchievementBoardPopup != null)
                AchievementBoardManager.Instance.currentAchievementBoardPopup.GetComponent<Popup>().Close();
            AchievementBoardManager.Instance.currentAchievementBoardPopup = m_popup;
            m_popup.GetComponent<AchievementBoardPopup>().SetAchievementBoardPopup(); // set up the achievement board popup
        }
    }
}