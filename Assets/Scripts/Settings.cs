using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    public static readonly Color highlightColor = new Color(1.0f, 0.33f, 0.64f, 1);
    public static readonly Color availableColor = new Color(0.56f, 1, 0.25f, 1);
    public static readonly Color selectedColor = new Color(1, 0.4f, 0.61f, 1);
    public static readonly Color darkBlue = new Color(0.1631843f, 0.247508f, 0.3960784f, 1);
    public static readonly Color green = new Color(0.007843138f, 0.5686275f, 0.2745098f, 1);

    public static readonly List<int> buildingLevel = new List<int> { 50, 100, 200, 400, 700, 1000, 1500, 2000, 3000, 4000, 5000 };
    public static readonly List<int> totalLevel = new List<int> { 263, 526, 789, 1053, 1316, 1579, 1842, 2105, 2368, 2631, 2895, 3158, 3421, 3684, 3947, 4210, 4473, 4736, 5000, 5500 };

    public static readonly List<Color> UIColors = new List<Color> {
        new Color(0.936f, 0.186264f, 0.37870947f, 1),
        new Color(1f, 0.36900002f, 0.5341611f, 1),
        new Color(1f, 0.52436954f, 0.14199996f, 1),
        new Color(1f, 0.80562246f, 0.11400002f, 1),
        new Color(0.011764706f, 0.7411765f, 0.35686275f, 1),
        new Color(0.08235294f, 0.6666667f, 0.7490196f, 1),
        new Color(0.19696076f, 0.6472748f, 0.9607843f, 1),
        new Color(0f, 0.54403514f, 0.886f, 1),
        new Color(0f, 0.54403514f, 0.609f, 1),
        new Color(0.6737891f, 0.22527999f, 0.88f, 1),
        new Color(0.45625788f, 0.2235294f, 0.8784314f, 1),
        new Color(0.31830707f, 0.4051181f, 0.525f, 1),
        new Color(0.16862746f, 0.23529412f, 0.34117648f, 1),
        new Color(0.150024f, 0.24041441f, 0.376f, 1),
        new Color(0.21960784f, 0.33333334f, 0.49019608f, 1),
        new Color(0.427651f, 0.5286514f, 0.654902f, 1),
        new Color(0.6784314f, 0.7254902f, 0.7882353f, 1),
        new Color(0.7882353f, 0.827451f, 0.8745098f, 1),
        new Color(0.92156863f, 0.9372549f, 0.9529412f, 1),
        new Color(1f, 1f, 1f, 1),
    };

    // legend colors
    public static readonly List<Color> legendColors = new List<Color>()
    {
        new Color(1f, 0f, 0f),           // rgb(255, 0, 0)
        new Color(245f / 255f, 13f / 255f, 5f / 255f),
        new Color(236f / 255f, 26f / 255f, 9f / 255f),
        new Color(226f / 255f, 39f / 255f, 14f / 255f),
        new Color(216f / 255f, 52f / 255f, 19f / 255f),
        new Color(207f / 255f, 66f / 255f, 23f / 255f),
        new Color(197f / 255f, 79f / 255f, 28f / 255f),
        new Color(188f / 255f, 92f / 255f, 33f / 255f),
        new Color(178f / 255f, 105f / 255f, 37f / 255f),
        new Color(168f / 255f, 118f / 255f, 42f / 255f),
        new Color(159f / 255f, 131f / 255f, 47f / 255f),
        new Color(149f / 255f, 144f / 255f, 52f / 255f),
        new Color(139f / 255f, 157f / 255f, 56f / 255f),
        new Color(130f / 255f, 170f / 255f, 61f / 255f),
        new Color(120f / 255f, 183f / 255f, 66f / 255f),
        new Color(111f / 255f, 197f / 255f, 70f / 255f),
        new Color(101f / 255f, 210f / 255f, 75f / 255f),
        new Color(91f / 255f, 223f / 255f, 80f / 255f),
        new Color(82f / 255f, 236f / 255f, 84f / 255f),
        new Color(72f / 255f, 249f / 255f, 89f / 255f)
    };

    public static int energyDivisor = 20000;
    public static float oldEnergyProvision = 6f;
}
