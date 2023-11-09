using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UltimateClean
{
    public class BuyButton : MonoBehaviour
    {
        TextMeshProUGUI itemPrice;
        protected ShopItem m_shopItem;
        public virtual void Buy()
        {
            itemPrice = transform.GetComponentInChildren<TextMeshProUGUI>();
            m_shopItem = transform.parent.GetComponent<ShopItemUI>().shopItem;
            ShopManager.Instance.currentShopItem = m_shopItem.Install(); // set the current shop item selected
            ShopManager.Instance.price = m_shopItem.currentPrice; // set the current price
            ShopManager.Instance.currentShopPopup.GetComponent<Popup>().Close();
            ShopManager.Instance.AddBackground();
            SemanticLayerManager.Instance.ShowSemanticLayerButtons(ShopManager.Instance.currentShopItem); // show the semantic layer btns corresponding to the selected item
        }
    }
}
