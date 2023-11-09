using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UltimateClean
{
    /// <summary>
    /// Player information panel controller
    /// </summary>
    public class InformationController : MonoBehaviour
    {

        public TextMeshProUGUI level;
        public TextMeshProUGUI playerName;
        public TextMeshProUGUI xp;


        private int m_level;

        void Start()
        {
            m_level = GameManager.Instance.level;
            if (level)
                level.text = "Lv. <#FF6573>" + m_level + " </color>";
            if (xp)
                xp.text = "<#00B6E4>" + GameManager.Instance.tempScore.ToString() + " </color>";
        }

        // Update is called once per frame
        void Update()
        {
            if (level)
            {
                if (m_level != GameManager.Instance.level)
                {
                    GetComponent<PopupOpener>()?.OpenPopup(); // open level up popup
                    m_level = GameManager.Instance.level;
                    level.text = "Lv. <#FF6573>" + m_level + " </color>";
                }
            }
        }
    }
}
