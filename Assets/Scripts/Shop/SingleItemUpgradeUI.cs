using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UltimateClean;

public class SingleItemUpgradeUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI currentLevel;
    [SerializeField] TextMeshProUGUI nextLevel;
    [SerializeField] TextMeshProUGUI cost;
    [SerializeField] InitContribution contributions;
    [SerializeField] ButtonWithSound upgradeButton;
    ShopItem item;
    Color defaultBtnColor;

    public void SetItemUI(ShopItem item)
    {
        this.item = item;
        defaultBtnColor = upgradeButton.transform.Find("Button (Color)").GetComponent<Image>().color;
        ShopItem shopItem = ShopManager.Instance.GetShopItem(item.name);
        icon.sprite = Resources.Load<Sprite>(item.icon);
        title.text = item.name;
        currentLevel.text = "Level " + item.currentLevel.ToString();
        if (item.currentLevel == shopItem.maxLevel)
        {
            nextLevel.text = "Max";
            cost.text = " - ??? -";
            DeactivateButton();
        }
        else
        {
            nextLevel.text = "Level " + (item.currentLevel + 1).ToString();
            cost.text = item.upgradePrice.ToString("N0");
            ActivateButton();
        }
        contributions.upgrade(item, SkillManager.Instance.getUserSkills()[item.name]);
        upgradeButton.onClick.AddListener(Pay);

    }

    void Pay()
    {
        if (GameManager.Instance.AddMoney(-item.upgradePrice))
        {
            item.UpgradeTo(item.currentLevel + 1);
            var buildings = ShopInstallManager.Instance.FindInfluencedBuildings(item) ?? new List<GameObject>();
            foreach (GameObject building in buildings)
            {
                ShopManager.Instance.UpdateBuilding(building, item);
            }
            SetItemUI(item);
            upgradeButton.onClick.RemoveAllListeners();
        }
    }

    /// <summary>
    /// deactivate this button
    /// </summary>
    void DeactivateButton()
    {
        upgradeButton.interactable = false;
        upgradeButton.transform.Find("Button (Color)").GetComponent<Image>().color = Color.gray;
    }

    /// <summary>
    /// activate button
    /// </summary>
    void ActivateButton()
    {
        upgradeButton.interactable = true;
        upgradeButton.transform.Find("Button (Color)").GetComponent<Image>().color = defaultBtnColor;
    }
}
