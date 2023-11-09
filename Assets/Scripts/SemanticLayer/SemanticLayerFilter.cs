using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SemanticLayerFilter : MonoBehaviour
{
    public TextMeshProUGUI value;
    public Image color;

    public void SetSemanticLayerFilter(Color color, string value)
    {
        this.color.color = color;
        this.value.text = value;
    }
}
