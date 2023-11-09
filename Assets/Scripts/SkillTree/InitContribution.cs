using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UltimateClean
{
    public class InitContribution : MonoBehaviour
    {
        private Skill m_skill;
        // Start is called before the first frame update
        public GameObject prefab;
        [SerializeField] Transform shopItemUI;
        private Dictionary<string, GameObject> objs = new Dictionary<string, GameObject>();
        void Start()
        {
            m_skill = shopItemUI.GetComponent<SkillUI>()?.GetSkill() ?? null;
            if (m_skill != null)
                upgrade();
        }

        public void upgrade()
        {
            ShopItem shopItem = ShopManager.Instance.GetShopItem(m_skill.getName());
            List<string> shopItemSemanticDatas = shopItem.semanticData;
            List<int> shopItemMaxContirbutions = shopItem.maxContribution;
            List<int> shopItemInitContirbutions = shopItem.contribution;

            for (int i = 0; i < shopItemSemanticDatas.Count; i++)
            {
                GameObject obj;
                if (!objs.ContainsKey(shopItemSemanticDatas[i]))
                {
                    obj = Instantiate(prefab, this.transform);

                }
                else
                {
                    obj = objs[shopItemSemanticDatas[i]];
                }
                Slider slider = obj.GetComponent<Slider>();
                int upgradedContribution = (shopItemMaxContirbutions[i] - shopItemInitContirbutions[i]) / (m_skill.getMaxLevel() - 1);
                int currentContribution = 0;
                if (m_skill.getCurrentLevel() != 0)
                    currentContribution = shopItemInitContirbutions[i] + (m_skill.getCurrentLevel() - 1) * upgradedContribution;

                slider.maxValue = shopItemMaxContirbutions[i];
                string Title = shopItemSemanticDatas[i] + " " + "<color=#FFD140>+" + (currentContribution + (m_skill.getCurrentLevel() == m_skill.getMaxLevel() ? 0 : upgradedContribution)) + " / " + shopItemMaxContirbutions[i] + "</color>";
                slider.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = Title;
                slider.value = currentContribution;

                Transform sliderBackground = slider.transform.Find("Slider background");
                sliderBackground.GetComponent<Slider>().maxValue = shopItemMaxContirbutions[i];
                if (m_skill.getCurrentLevel() == m_skill.getMaxLevel())
                    sliderBackground.GetComponent<Slider>().value = 0;
                else
                    sliderBackground.GetComponent<Slider>().value = currentContribution + upgradedContribution;
                objs[shopItemSemanticDatas[i]] = obj;
            }
        }

        public void upgrade(ShopItem shopItem, Skill skill)
        {
            List<string> shopItemSemanticDatas = shopItem.semanticData;
            List<int> shopItemMaxContirbutions = shopItem.maxContribution;
            List<int> shopItemInitContirbutions = shopItem.contribution;

            for (int i = 0; i < shopItemSemanticDatas.Count; i++)
            {
                GameObject obj;
                if (!objs.ContainsKey(shopItemSemanticDatas[i]))
                {
                    obj = Instantiate(prefab, this.transform);
                    obj.transform.localScale = new Vector3(3.3f, 3.3f, 0);

                }
                else
                {
                    obj = objs[shopItemSemanticDatas[i]];
                }
                Slider slider = obj.GetComponent<Slider>();
                int upgradedContribution = (shopItemMaxContirbutions[i] - shopItemInitContirbutions[i]) / (skill.getMaxLevel() - 1);
                int currentContribution = 0;
                if (shopItem.currentLevel != 0)
                    currentContribution = shopItemInitContirbutions[i] + (shopItem.currentLevel - 1) * upgradedContribution;

                slider.maxValue = shopItemMaxContirbutions[i];
                string Title = shopItemSemanticDatas[i] + " " + "<color=#FFD140>+" + (currentContribution + (shopItem.currentLevel == skill.getMaxLevel() ? 0 : upgradedContribution)) + " / " + shopItemMaxContirbutions[i] + "</color>";
                slider.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = Title;
                slider.value = currentContribution;

                Transform sliderBackground = slider.transform.Find("Slider background");
                sliderBackground.GetComponent<Slider>().maxValue = shopItemMaxContirbutions[i];
                if (shopItem.currentLevel == skill.getMaxLevel())
                    sliderBackground.GetComponent<Slider>().value = 0;
                else
                    sliderBackground.GetComponent<Slider>().value = currentContribution + upgradedContribution;
                objs[shopItemSemanticDatas[i]] = obj;
            }
        }
    }
}