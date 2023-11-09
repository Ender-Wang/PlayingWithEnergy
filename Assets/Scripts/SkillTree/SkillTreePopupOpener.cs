using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateClean{
public class SkillTreePopupOpener : PopupOpener
{
    public override void OpenPopup()
    {
        base.OpenPopup();
        if (SkillManager.Instance.currentSkillTreePopup != null)
            SkillManager.Instance.currentSkillTreePopup.GetComponent<Popup>().Close();
        SkillManager.Instance.currentSkillTreePopup = m_popup;
        m_popup.GetComponent<SkillPopup>().SetSkillPopup(); // set up the skill popup
    }
}
}