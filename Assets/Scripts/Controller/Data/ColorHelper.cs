using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This class is a utillity class for colors.
/// </summary>
public class ColorHelper
{
    public static float GetColorDistance(Color c1, Color c2)
    {
        float r = c1.r - c2.r;
        float g = c1.g - c2.g;
        float b = c1.b - c2.b;

        return Mathf.Sqrt(r * r + g * g + b * b);
    }

    // get colors from Settings.legendColors, from index "from" but will skip some colors
    public static Color[] GetColors(int count)
    {

        int from = 0;
        int step = Settings.legendColors.Count / count;
        int to = Settings.legendColors.Count - 1;

        // get colors
        Color[] colors = new Color[count];
        Stack<int> myStack = new Stack<int>();
        for (int i = 0; i < count; i++)
        {
            int index = from + i * step;
            colors[i] = Settings.legendColors[index];
            myStack.Push(index);
        }

        colors[colors.Length - 1] = Settings.legendColors[to];

        return colors;
    }

    // get colors from Settings.legendColors, from index "from" but will skip some colors
    public static Color getColorByDataSetting(Color colorInPic, DataSetting dataSetting)
    {
        float min = 0.3f;
        int index = 0; // default color
        foreach (Color colorInOriginColors in dataSetting.originColors)
        {
            float distance = GetColorDistance(colorInPic, colorInOriginColors);

            if (distance < min)
            {
                index = dataSetting.originColors.IndexOf(colorInOriginColors);
                min = distance;
            }
        }
        return dataSetting.colors[index];
    }

    // get colors from Settings.legendColors, according to the originInfo
    public static Color getColorByDataSetting(string originInfo, DataSetting dataSetting)
    {
        int index = dataSetting.originInfos.IndexOf(originInfo);
        if (index == -1)
        {
            Debug.Log("No such displayedInfo: " + originInfo);
            return dataSetting.colors[0];
        }
        return dataSetting.colors[index];
    }
}