using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

namespace UltimateClean
{
    public class SkillUI : PopupOpener
    {
        public Image icon;
        public TextMeshProUGUI skillName;
        public TextMeshProUGUI skillDescription;
        public TextMeshProUGUI cost;
        public Skill skill { get; set; }
        public ButtonWithSound button;
        public SkillProgressAnimation progressbar;
        public TextMeshProUGUI currentLevel;
        public TextMeshProUGUI skillProgressPercentage;
        [SerializeField] InitContribution contributionParent;

        private Dictionary<string, GameObject> contributions;
        public GameObject contributionPrefab;

        public Skill GetSkill()
        {
            return this.skill;
        }

        /// <summary>
        /// prepare skill item (detailed information) for the given <param name="skill"></param>
        /// </summary>
        /// <param name="skill"></param>
        public void SetSkillUI(Skill skill)
        {
            this.skill = skill;
            this.progressbar.skill = skill;
            skillName.text = skill.getName();
            cost.text = skill.getCost().ToString("N0");
            skillDescription.text = skill.description;
            int count = skill.getPrerequisites().Count;
            string prerequisites = "\nThis technologie has " + count + " prerequirement(s): ";
            foreach (Skill s in skill.getPrerequisites())
            {
                prerequisites += s.getName() + ", ";
            }
            prerequisites = prerequisites.Remove(prerequisites.Length - 2);
            skillDescription.text += prerequisites;
            Sprite skillIcon = Resources.Load<Sprite>(skill.iconPath);
            if (skillIcon)
                icon.sprite = skillIcon;
            // if skill is onLearning state, should dynamically set button avaliability(call ActivateButton())
            if (this.skill.isOnLearning())
            {
                DeactivateButton();
                StartCoroutine(SkillReady(this.skill.TimeNeedToFinish()));
                StartCoroutine(progressbar.AnimateProgress());
            }
            else
            {
                skillProgressPercentage.text = "0%";
                progressbar.gameObject.GetComponent<SlicedFilledImage>().fillAmount = 0f;
            }
            if (this.skill.getCurrentLevel() == this.skill.getMaxLevel())
            {
                DeactivateButton();
                currentLevel.text = "Max";
                skillProgressPercentage.text = "100%";
                cost.text = "∞";
                cost.fontStyle = FontStyles.Bold;
                cost.fontSize = 40f;
                progressbar.gameObject.GetComponent<SlicedFilledImage>().fillAmount = 1f;
            }
            else
            {
                currentLevel.text = "Current level: " + skill.getCurrentLevel();
                cost.text = skill.getCost().ToString("N0");
            }
        }

        /// <summary>
        /// function is called when user click on the learn skill button
        /// </summary>
        public void LearningSkill()
        {
            // TODO: money withdraw
            Dictionary<int, string> res = skill.StartDevelop();
            base.OpenPopup();
            if (res.ContainsKey(200))
            {
                DeactivateButton();
                base.m_popup.transform.Find("Logo").Find("Success").gameObject.SetActive(true);
                base.m_popup.transform.Find("LearningResultText").gameObject.GetComponent<TextMeshProUGUI>().text = res[200];

                SkillManager.Instance.invokeSkillReady(skill.TimeNeedToFinish(), skill);
                GameManager.Instance.AddEnergy(-this.skill.getCost());
                StartCoroutine(SkillReady(skill.TimeNeedToFinish()));
                StartCoroutine(progressbar.AnimateProgress());
            }
            else
            {
                foreach (KeyValuePair<int, string> kvp in res)
                {
                    base.m_popup.transform.Find("Logo").Find("Fail").gameObject.SetActive(true);
                    base.m_popup.transform.Find("LearningResultText").gameObject.GetComponent<TextMeshProUGUI>().text = kvp.Value;
                }
            }
        }

        /// <summary>
        /// this coroutine is used to set the skill available, once it finishs developing
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        IEnumerator SkillReady(float seconds)
        {
            // TODO: add global message notification
            yield return new WaitForSeconds(seconds);
            if (skill.getCurrentLevel() < skill.getMaxLevel())
            {
                ActivateButton();
                currentLevel.text = "Current level: " + skill.getCurrentLevel();
                cost.text = skill.getCost().ToString();
                skillProgressPercentage.text = "0%";
                progressbar.gameObject.GetComponent<SlicedFilledImage>().fillAmount = 0f;
            }
            else
            {
                currentLevel.text = "Current level: Max";
                cost.text = "∞";
                cost.fontStyle = FontStyles.Bold;
                cost.fontSize = 40f;
            }
            contributionParent?.upgrade();
        }

        /// <summary>
        /// if LearningSkill() success, deactivate this button
        /// </summary>
        void DeactivateButton()
        {
            button.interactable = false;
            button.transform.Find("Button (Color)").GetComponent<Image>().color = Color.gray;
        }

        /// <summary>
        /// if skill's currentlevel < maxLevel, activate button
        /// </summary>
        void ActivateButton()
        {
            button.interactable = true;
            button.transform.Find("Button (Color)").GetComponent<Image>().color = new Color(0f, 0.4196078f, 0.6784314f, 1f);
        }
    }
}