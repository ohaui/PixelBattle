using System.Drawing;

namespace PixelBattleFront.Extensions;

public static class ColorExtension
{
    public static string ToHex(this Color color)
    {
        return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
    }
}