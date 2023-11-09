using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class KeyBoardUI : FollowingUI
{

    public override FollowingUI Handle(string request)
    {
        type = FollowingType.Keyboard;
        if (UI == null)
        {
            UI = UIs["KeyboardUI"];
            UI = Instantiate(UI);
            UI.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = request;
        }
        StartCoroutine(Animate());
        return this;
    }

    public override bool IsAllowed(FollowingType type)
    {
        return type == FollowingType.Keyboard;
    }
}
