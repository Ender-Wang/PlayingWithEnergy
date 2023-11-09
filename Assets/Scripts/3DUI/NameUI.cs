using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class NameUI : FollowingUI
{

    public override FollowingUI Handle(string request)
    {
        type = FollowingType.Name;
        if (UI == null)
        {
            // TODO: Optimize
            UI = UIs["NameUI"];
            UI = Instantiate(UI);
            UI.GetComponent<TextMeshProUGUI>().text = request.Replace("[", "").Replace("]", "").Replace("(Clone)", "");
        }
        return this;
    }

    public override bool IsAllowed(FollowingType type)
    {
        return type == FollowingType.Name;
    }
}
