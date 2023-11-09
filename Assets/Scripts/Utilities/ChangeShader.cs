using EPOOutline;
using UnityEngine;

public class ChangeShader : MonoBehaviour
{
    private Outlinable outlinable;

    void Start()
    {
        outlinable = GetComponentInChildren<Outlinable>();
        outlinable.OutlineLayer = 1;
    }

    public void Select()
    {
        outlinable.enabled = true;
    }

    public void DisSelect()
    {
        outlinable.enabled = false;
    }

    public void ChangeColor(Color color)
    {
        outlinable.OutlineParameters.Color = color;
    }

    public bool ShaderIsOn() { return outlinable.enabled; }
    public Color GetShaderColor() { return outlinable.OutlineParameters.Color; }
}
