using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public enum FollowingType
{
    None,
    Name,
    Mark,
    Keyboard,
}

/// <summary>
/// Chain of responsibility pattern for creating UI for objects
/// </summary>
public class FollowingUI : MonoBehaviour
{
    private FollowingUI next;
    public GameObject UI;
    public FollowingType type;
    protected Dictionary<string, GameObject> UIs;

    void Awake()
    {
        UIs = new Dictionary<string, GameObject>();
        foreach (var UI in Resources.LoadAll<GameObject>("UI Resources/ObjectWithUI"))
        {
            UIs.Add(UI.name, UI);
        }
    }

    public FollowingUI Add(FollowingUI next)
    {
        this.next = next;
        return next;
    }

    /// <summary>
    /// Main method for creating UI
    /// </summary>
    /// <param name="type"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public FollowingUI Create(FollowingType type, string request = "")
    {
        if (IsAllowed(type))
        {
            return Handle(request);
        }
        else return next?.Create(type, request) ?? null;
    }

    /// <summary>
    /// handle the request and create the UI
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public virtual FollowingUI Handle(string request) { return null; }

    /// <summary>
    /// Verify if the UI is allowed to be created
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public virtual bool IsAllowed(FollowingType type) { return false; }

    protected IEnumerator Animate()// animate the current UI (punching effect)
    {
        while (true)
        {
            UI.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 3f, 3, 0);
            yield return new WaitForSeconds(4f);
        }
    }
}
