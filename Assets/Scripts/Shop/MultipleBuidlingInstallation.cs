using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateClean
{
    public class MultipleBuidlingInstallation : BuyButton
    {

        public override void Buy()
        {
            base.Buy();
            MultiSelectController.Instance.enabled = true; // enable the multi select controller
            SelectingController.Instance.enabled = false; // disable the single select controller
            ShopManager.Instance.startShopping = true; // start shopping
            MultiSelectController.Instance.CheckAvailableBuildings();
        }


    }
}
