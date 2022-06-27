using UnityEngine;

public static class ColorConvertor
{
    public static Color ConvertHEX(string value)
    {
        //#3a86ff
        string R = value.Substring(1, 2);
        string G = value.Substring(3, 2);
        string B = value.Substring(5, 2);

        float sR = (float)int.Parse(R, System.Globalization.NumberStyles.AllowHexSpecifier) / 255f;
        float sG = (float)int.Parse(G, System.Globalization.NumberStyles.AllowHexSpecifier) / 255f;
        float sB = (float)int.Parse(B, System.Globalization.NumberStyles.AllowHexSpecifier) / 255f;

        return new Color(sR, sG, sB);
    }
}