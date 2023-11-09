using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateClean
{
    public class ConfirmBuyingPanelOpener : PopupOpener
    {
        public override void OpenPopup()
        {
            base.OpenPopup();
            ShopManager.Instance.OpenPopup(m_popup);
            m_popup.GetComponent<ConfirmBuyingPanelUI>().SetConfirmBuyingPanelUI(PriceCalculator().ToString()); // set up the confirmBuying popup
        }

        /// <summary>
        /// Calculate the total price of the selected buildings
        /// </summary>
        /// <returns>total price</returns>
        private float PriceCalculator()
        {
            if (MultiSelectController.Instance.enabled)
            {
                float price = 0;
                foreach (GameObject building in MultiSelectController.Instance.selectedBuildings)
                {
                    price += ShopManager.Instance.price * (int)BuildingManager.Instance.GetBuildingState(BuildingManager.Instance.GetBuildingName(building)).GetSize().x * (int)BuildingManager.Instance.GetBuildingState(BuildingManager.Instance.GetBuildingName(building)).GetSize().z;
                }
                return price;
            }
            else
                return ShopManager.Instance.price;
        }

    }
}
