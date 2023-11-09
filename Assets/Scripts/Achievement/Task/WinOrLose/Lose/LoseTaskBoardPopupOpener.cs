using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UltimateClean
{
    public class LoseTaskBoardPopupOpener : PopupOpener
    {
        private string m_loseReason;
        public override void OpenPopup()
        {
            base.OpenPopup();
            m_popup.GetComponent<LoseTaskBoardPopup>().SetLoseTaskPopup(m_loseReason);
        }

        public void SetLoseReason(string loseReason)
        {
            m_loseReason = loseReason;
            OpenPopup();
        }
    }
}