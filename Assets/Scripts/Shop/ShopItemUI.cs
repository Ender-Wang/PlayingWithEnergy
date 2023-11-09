using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UltimateClean
{
    public class ShopItemUI : MonoBehaviour
    {
        public Image itemIcon;
        public TextMeshProUGUI itemCategory;
        public TextMeshProUGUI currentLevel;
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI itemToolTipName;
        public TextMeshProUGUI itemDescription;
        public TextMeshProUGUI itemPrice;
        public GameObject itemContributionPrefab;
        public GameObject itemContributionParent;
        public TMP_Dropdown itemLevel;
        public GameObject selectButton;
        [SerializeField] TextMeshProUGUI amountText;
        public ShopItem shopItem { get; set; }
        private int maxLevel = 0;
        private List<ContributionUI> contributionsUI;
        private bool canChangeLevel = false; // avoid changing value when the dropdown is not ready

        public void SetShopItemUI(ShopItem shopItem)
        {
            if (currentLevel != null)
                currentLevel.text = "Lv." + shopItem.currentLevel.ToString();
            if (shopItem.icon != null)
                itemIcon.sprite = Resources.Load<Sprite>(shopItem.icon);
            if (amountText != null)
                amountText.text = shopItem.numberOfItems.ToString();
            // itemCategory.text = shopItem.category;
            itemName.text = shopItem.name;
            itemToolTipName.text = shopItem.name;
            itemDescription.text = shopItem.description;
            itemPrice.text = shopItem.currentPrice.ToString("N0");
            // itemContribution.text = contribution;
            this.shopItem = shopItem;
            this.maxLevel = shopItem.maxLevel;
            if (selectButton)
            {
                if (shopItem.range == 0)
                {
                    selectButton.AddComponent<MultipleBuidlingInstallation>();
                    selectButton.GetComponent<ButtonWithSound>().onClick.AddListener(selectButton.GetComponent<MultipleBuidlingInstallation>().Buy);
                }
                else
                {
                    selectButton.AddComponent<SingleBuildingInstallation>();
                    selectButton.GetComponent<ButtonWithSound>().onClick.AddListener(selectButton.GetComponent<SingleBuildingInstallation>().Buy);
                }
            }
            // shopItem.ResetItem(); // initialize the shop item


            if (this.maxLevel >= 1 && shopItem.range == 0) // multiple items installation
                SetLevelDropDown();
            if (shopItem.range != 0)
            { // single item installation
                itemLevel.gameObject.SetActive(false);
            }
            contributionsUI = new List<ContributionUI>();
            int j = 0;
            for (int i = 0; i < shopItem.currentContributions.Count; i++)
            {
                var contribution = Instantiate(itemContributionPrefab, itemContributionParent.transform, false);
                Color color1 = Settings.UIColors[j % Settings.UIColors.Count];
                Color color2 = Settings.UIColors[(j + 1) % Settings.UIColors.Count];
                contribution.GetComponentInChildren<ContributionUI>().SetContributionUI(shopItem.semanticData[i], shopItem.currentContributions[i], shopItem.maxContribution[i], color1, color2);
                contributionsUI.Add(contribution.GetComponentInChildren<ContributionUI>());
                j += 2;
            }

        }

        private void SetLevelDropDown()
        {
            for (int i = 0; i < this.maxLevel; i++)
            {
                TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
                data.text = "Level " + (i + 1).ToString();
                // itemContribution.options.Add(data);
                itemLevel.options.Add(data);
            }
            itemLevel.value = shopItem.currentLevel - 1;
            canChangeLevel = true;
        }

        public void DropdownValueChanged(TMP_Dropdown change)
        {
            if (canChangeLevel)
            {
                shopItem.UpgradeTo(change.value + 1);
                itemPrice.text = shopItem.currentPrice.ToString("N0");
                // change the contribution
                for (int i = 0; i < contributionsUI.Count; i++)
                {
                    contributionsUI[i].ChangeContribution(shopItem.currentContributions[i], shopItem.maxContribution[i]);
                }
            }
        }
    }
}