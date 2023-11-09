using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Change canvas camera to this camera
/// </summary>
[RequireComponent(typeof(Camera))]
public class ChangeCanvasCamera : MonoBehaviour
{
    void OnEnable()
    {
        Debug.Log(GameManager.Instance);
        GameManager.Instance.canvas.worldCamera = GetComponent<Camera>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(GameManager.Instance.canvas.GetComponent<RectTransform>());
    }
}
