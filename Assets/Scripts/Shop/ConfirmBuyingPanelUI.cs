using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UltimateClean
{
    public class ConfirmBuyingPanelUI : MonoBehaviour
    {
        public TextMeshProUGUI itemPrice;
        public TextMeshProUGUI buildingsAmount;
        public GameObject selectedItem;
        public TextMeshProUGUI itemLevel;
        public GameObject notificationPrefab;

        private float m_price;
        private List<GameObject> m_buildings;
        private ShopItem m_shopItem;

        public void SetConfirmBuyingPanelUI(string price)
        {
            m_shopItem = ShopManager.Instance.currentShopItem;
            itemLevel.text = "Level " + m_shopItem.currentLevel.ToString();
            selectedItem.GetComponent<ShopItemUI>().SetShopItemUI(m_shopItem);
            m_buildings = new List<GameObject>();
            if (MultiSelectController.Instance.enabled)
            {
                buildingsAmount.text = MultiSelectController.Instance.selectedBuildings.Count.ToString() + " buildings will be installed with " + m_shopItem.name;
                foreach (GameObject building in MultiSelectController.Instance.selectedBuildings)
                {
                    m_buildings.Add(building);
                }
            }
            else
            {
                buildingsAmount.text = m_shopItem.name + " will be installed";
                foreach (GameObject building in SingleInstallController.Instance.selectedBuildings)
                {
                    m_buildings.Add(building);
                }
            }
            m_price = float.Parse(price);
            itemPrice.text = "<color=#FCEB68>" + m_price.ToString("N0") + "</color>";

            // MultiSelectController.Instance.enabled = false; // disable the multi select controller
        }

        /// <summary>
        /// Button click event
        /// </summary>
        public void Cancel()
        {
            if (SingleInstallController.Instance.enabled)
                SingleInstallController.Instance.Restart(false);
            ClosePopup();
        }

        /// <summary>
        /// Button click event
        /// </summary>
        public void Pay()
        {
            ShopItem shopItem = m_shopItem.Install();
            if (GameManager.Instance.AddMoney(-m_price))
            { // pay the price
                UpdateBuildings(shopItem); // update the buildings with the shop item
                if (MultiSelectController.Instance.enabled)
                    MultiSelectController.Instance.CheckAvailableBuildings();
                if (SingleInstallController.Instance.enabled)
                {
                    ShopInstallManager.Instance.AddShopItem(shopItem); // save to the list of shop items installed
                    SingleInstallController.Instance.Restart(true);
                }
                SemanticLayerManager.Instance.currentSemanticLayerButton.GetComponent<SemanticLayerButton>().button.onClick.Invoke();
                SemanticLayerManager.Instance.currentSemanticLayerButton.GetComponent<SemanticLayerButton>().button.onClick.Invoke(); // refresh the color by clicking twice the semantic layer button
                ClosePopup();
            }
        }

        private void UpdateBuildings(ShopItem shopItem)
        {
            if (MultiSelectController.Instance.enabled)
            {
                foreach (GameObject building in m_buildings)
                {

                    ShopManager.Instance.UpdateBuilding(building, shopItem.Install((int)BuildingManager.Instance.GetBuildingState(BuildingManager.Instance.GetBuildingName(building)).GetSize().x * (int)BuildingManager.Instance.GetBuildingState(BuildingManager.Instance.GetBuildingName(building)).GetSize().z));
                }
            }

            if (SingleInstallController.Instance.enabled)
            {
                foreach (GameObject building in m_buildings)
                {
                    ShopManager.Instance.UpdateBuilding(building, shopItem);
                }
            }
        }

        /// <summary>
        /// Integrate the actions before closing the popup
        /// </summary>
        private void ClosePopup()
        {
            ShopManager.Instance.startShopping = true; // start multiple building installation
            m_buildings.Clear();
            if (MultiSelectController.Instance.enabled)
                StartCoroutine(MultiSelectController.Instance.DisSelectBuildings());
            if (SingleInstallController.Instance.enabled)
                StartCoroutine(SingleInstallController.Instance.DisSelectBuildings());
            GetComponent<Popup>().Close();
        }
    }
}
