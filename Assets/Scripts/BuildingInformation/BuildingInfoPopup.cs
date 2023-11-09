using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace UltimateClean
{
    public class BuildingInfoPopup : MonoBehaviour
    {
        public TextMeshProUGUI buildingName;
        public Image progressBar;
        public GameObject semanticDataPrefab;
        public GameObject shopItemPrefab;
        public Sprite sematicDataIcon;
        public TextMeshProUGUI experienceTooltip;
        public TextMeshProUGUI currentLevel;
        [SerializeField] TextMeshProUGUI buildingSizeText;
        [SerializeField] TextMeshProUGUI buildingSizeInDetailText;
        [SerializeField] TextMeshProUGUI buildingEnergyProvisionText;
        [SerializeField] TextMeshProUGUI buildingOldEnergyProvisionText;
        public Transform dataParent; // where the semantic data UI is located
        public Transform itemParent; // where the shop item UI is located



        protected GameObject m_semanticData;
        protected string m_contribution = "0";
        protected string m_semanticDataValue = "";
        protected string m_semanticDataName = "";
        protected string m_contributeName = "";


        /// <summary>
        /// Configure the popup with the information of the building
        /// </summary>
        /// <param name="buildingInfo">the building's information provided</param>
        public void SetBuildingInfoPopup(Dictionary<string, Dictionary<string, string>> buildingInfo, Building building)
        {
            buildingName.text = building.GetName();
            // set up the building information
            KeyValuePair<int, int> levelInfo = BuildingManager.Instance.GetBuildingLevel(building);
            experienceTooltip.text = levelInfo.Value + " / " + Settings.buildingLevel[levelInfo.Key];
            currentLevel.text = "Level " + levelInfo.Key;
            progressBar.fillAmount = (float)Math.Round((decimal)levelInfo.Value / Settings.buildingLevel[levelInfo.Key], 2); // remain 2 decimal places

            // set up the building size
            float x = (BuildingManager.Instance.buildingObjects[building].GetComponent<BoxCollider>()?.size.x ?? 0) * 100;
            float y = (BuildingManager.Instance.buildingObjects[building].GetComponent<BoxCollider>()?.size.y ?? 0) * 100;
            float z = (BuildingManager.Instance.buildingObjects[building].GetComponent<BoxCollider>()?.size.z ?? 0) * 100;
            float roofSize = x * z;
            buildingSizeText.text = roofSize.ToString("F2");
            buildingSizeInDetailText.text = x.ToString("F2") + " * " + y.ToString("F2") + " * " + z.ToString("F2");

            // set up the items which are installed in the building
            foreach (KeyValuePair<int, ShopItem> item in building.GetItemsInstalled())
            {
                GameObject itemPrefab = Instantiate(shopItemPrefab, itemParent, false);
                itemPrefab.GetComponent<ShopItemUI>().SetShopItemUI(item.Value);
            }
            // string info = "";
            foreach (KeyValuePair<string, Dictionary<string, string>> sematicData in buildingInfo)
            {
                m_semanticData = Instantiate(semanticDataPrefab, dataParent, false);
                // info += "Semantic: " + sematicData.Key + "\n";
                foreach (KeyValuePair<string, string> data in sematicData.Value)
                {
                    // info += " -" + data.Key + ": " + data.Value + "\n";
                    if (data.Key == "Contribution")
                    {
                        m_contribution = data.Value;
                    }
                    else if (data.Key == "LevelFactor")
                    {
                        m_contributeName = data.Value;
                    }
                    else
                    {
                        m_semanticDataName = data.Key;
                        m_semanticDataValue = data.Value;
                        sematicDataIcon = Resources.Load<Sprite>("EnergyData/" + data.Key + "/" + data.Key);
                    }
                }
                m_semanticData.GetComponent<SemanticDataUI>().SetSemanticDataUI(m_semanticDataName, m_semanticDataValue, m_contribution, m_contributeName, sematicDataIcon);
            }

            buildingEnergyProvisionText.text = building.GetTotalEnergyProvision().ToString("N0");
            buildingOldEnergyProvisionText.text = building.GetOldEnergyProvision().ToString("0.00");
        }
    }
}
