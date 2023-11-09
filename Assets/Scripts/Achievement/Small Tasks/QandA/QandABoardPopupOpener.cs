using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateClean
{
    public class QandABoardPopupOpener : PopupOpener
    {
        public static QandABoardPopupOpener Instance;
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public override void OpenPopup()
        {
            base.OpenPopup();
            if (QandAManager.Instance.currentQandAPopup != null)
                QandAManager.Instance.currentQandAPopup.GetComponent<Popup>().Close();
            QandAManager.Instance.currentQandAPopup = m_popup;
            m_popup.GetComponent<QandABoardPopup>().SetQandAPopup(); // set up the achievement board popup
        }
    }

}
