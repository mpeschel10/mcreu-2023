using UnityEngine;

public class PillarColor
{
    public static Color HeightToColor(float height)
    {
        // assume height [0, 1]
        // height *= height;
        float hue = height * 0.75f; // Looks better if highest point is purple instead of cycling back to red
        float saturation = 0.7f + height * 0.3f;
        float value = 0.25f + height * 0.25f;
        return Color.HSVToRGB(hue, saturation, value);
    }
}