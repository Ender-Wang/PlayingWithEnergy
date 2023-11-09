using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateClean
{
    public class SingleBuildingInstallation : BuyButton
    {

        public override void Buy()
        {
            base.Buy();
            SingleInstallController.Instance.enabled = true; // enable the single building installation controller
            SelectingController.Instance.enabled = false; // disable the single select controller
            SingleInstallController.Instance.SetRange(m_shopItem.range); // set the range of the single building installation controller
            ShopManager.Instance.startShopping = true; // start shopping
            SingleInstallController.Instance.CheckAvailableBuildings();
        }


    }
}
