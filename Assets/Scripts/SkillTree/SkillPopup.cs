using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateClean
{
    public class SkillPopup : MonoBehaviour
    {
        public GameObject skillPrefab;
        public Transform skillParent;

        private GameObject m_skill;


        /// <summary>
        /// Set the skill popup with the skill items from the skill manager
        /// </summary>
        public void SetSkillPopup()
        {
            foreach (KeyValuePair<string, Skill> kvp in SkillManager.Instance.getUserSkills())
            {
                m_skill = Instantiate(skillPrefab, skillParent, false);
                m_skill.GetComponent<SkillUI>().SetSkillUI(kvp.Value);
            }
        }
    }
}
