using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UltimateClean
{
    public class OpenToolTips : PopupOpener
    {
        public GameObject parent;

        public TextAsset toolTipContentFile;

        [System.Serializable]
        public class ToolTipsContent
        {
            public string Achievement;
            public string TechTree;
        }
        public override void OpenPopup()
        {
            m_popup = Instantiate(popupPrefab, parent.transform, false);
            m_popup.SetActive(true);
            m_popup.transform.localScale = Vector3.zero;
            m_popup.GetComponent<Popup>().Open();
        }

        /// <summary>
        /// This method is used to open the tool tip popup with the title.
        /// </summary>
        /// <param name="title">The EXACT key name defined in ToolTipsContent.json located in ./Resources/ToolTips Resource/ToolTipsContent.json.</param>
        public void OpenPopup(string title)
        {
            m_popup = Instantiate(popupPrefab, parent.transform, false);
            m_popup.SetActive(true);
            m_popup.transform.localScale = Vector3.zero;
            m_popup.GetComponent<Popup>().Open();

            TextMeshProUGUI toolTipTitle = m_popup.transform.Find("Top").Find("Title").GetComponent<TMPro.TextMeshProUGUI>();
            TextMeshProUGUI toolTipContent = m_popup.transform.Find("ScrollView").Find("Viewport").Find("Content").Find("Text").GetComponent<TMPro.TextMeshProUGUI>();

            ToolTipsContent toolTipContents = JsonUtility.FromJson<ToolTipsContent>(toolTipContentFile.text);
            toolTipTitle.text = title;

            if (title == "Achievement")
            {
                toolTipContent.text = toolTipContents.Achievement;
            }
            else if (title == "TechTree")
            {
                toolTipContent.text = toolTipContents.TechTree;
            }
            else
            {
                Debug.LogError("The tool tip title is not found");
            }
        }
    }
}

