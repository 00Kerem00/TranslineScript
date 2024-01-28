using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralVariables : MonoBehaviour
{
    public static string[] hexColors = { "#003DFF", "#02FF00", "#FFD500", "#FF5100", "#8800FF", "#FFFFFF", "#FF00D7" };
    public static string[] UIHexColors = { "#0006FF", "#00571A", "#546600", "#721600", "#570061", "#767676", "#FF00D7" };
    public static Color GetEraUIColor(int era)
    {
        return Holor(UIHexColors[era]);
    }
    public static Color GetEraColor(int era)
    {
        return Holor(hexColors[era]);
    }
    public static Color GetRandomColor()
    {
        Color result = Holor(hexColors[Random.Range(0, 1)]);
        return result;
    }

    static Color Holor(string hexColor)
    {
        Color color = new Color();

        ColorUtility.TryParseHtmlString(hexColor, out color);

        return color;
    }
}
