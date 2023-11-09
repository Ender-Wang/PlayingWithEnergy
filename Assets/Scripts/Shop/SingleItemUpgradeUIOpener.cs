using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateClean
{
    public class SingleItemUpgradeUIOpener : PopupOpener
    {
        // do not use this function directly
        public override void OpenPopup()
        {
            base.OpenPopup();
        }

        // open the popup with information of the selected building
        public void OpenPopup(Transform infrastructure)
        {
            OpenPopup();
            ShopItem item = ShopInstallManager.Instance.GetShopItem(infrastructure);
            m_popup.GetComponent<SingleItemUpgradeUI>().SetItemUI(item); // set up the information of the building into the popup
        }
    }
}
