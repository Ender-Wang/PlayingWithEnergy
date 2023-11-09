using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UltimateClean
{
    public class ShopPopup : MonoBehaviour
    {
        public GameObject shopItemPrefab;
        public Transform shopItemParent;

        private GameObject m_shopItem;


        /// <summary>
        /// Set the shop popup with the shop items from the shop manager
        /// </summary>
        public void SetShopPopup()
        {
            foreach (ShopItem item in ShopManager.Instance.shopItems)
            {
                if (item.isVisible)
                {
                    m_shopItem = Instantiate(shopItemPrefab, shopItemParent, false);
                    m_shopItem.GetComponent<ShopItemUI>().SetShopItemUI(item);
                }

            }
        }
    }
}
