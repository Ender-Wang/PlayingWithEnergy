using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MarkUI : FollowingUI
{
    public override FollowingUI Handle(string request)
    {
        type = FollowingType.Mark;
        if (UI == null)
        {
            UI = UIs["MarkUI"];
            UI = Instantiate(UI);
        }
        StartCoroutine(Animate());
        return this;
    }

    public override bool IsAllowed(FollowingType type)
    {
        return type == FollowingType.Mark;
    }


}
