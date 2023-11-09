using System;
using System.Collections.Generic;
using UnityEngine;


namespace UltimateClean
{
    public class ShopPopupOpener : PopupOpener
    {


        public override void OpenPopup()
        {
            base.OpenPopup();
            if (ShopManager.Instance.currentShopPopup != null)
                ShopManager.Instance.currentShopPopup.GetComponent<Popup>().Close();
            ShopManager.Instance.currentShopPopup = m_popup;
            m_popup.GetComponent<ShopPopup>().SetShopPopup(); // set up the shop popup
        }

    }
}