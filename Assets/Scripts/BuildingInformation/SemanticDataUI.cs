using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UltimateClean
{
    public class SemanticDataUI : MonoBehaviour
    {
        public Image semanticDataIcon;
        public TextMeshProUGUI semanticDataName;
        public TextMeshProUGUI semanticDataValue;
        public TextMeshProUGUI semanticDataContribution;
        public TextMeshProUGUI contributionName;

        /// <summary>
        /// Configure the semantic data UI with the information of the building
        /// </summary>
        /// <param name="name">semantic data name</param>
        /// <param name="value">semantic data value</param>
        /// <param name="contribution">semantic data contribution to heat</param>
        /// <param name="icon">semantic data icon</param>
        public void SetSemanticDataUI(string name, string value, string contribution, string contributionName, Sprite icon)
        {
            semanticDataName.text = name;
            semanticDataValue.text = value;
            semanticDataContribution.text = contribution;
            this.contributionName.text = contributionName + " Contribution";
            semanticDataIcon.sprite = icon;
        }
    }
}
