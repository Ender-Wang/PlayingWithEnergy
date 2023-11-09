using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCanvasMode : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.canvas.renderMode = RenderMode.ScreenSpaceCamera;
        GameManager.Instance.canvas.worldCamera = Camera.main;
    }

    private void OnDisable()
    {
        GameManager.Instance.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}
