using Archipelago.MultiClient.Net.Models;

namespace CornKidzAP.Archipelago;

public static class Utils
{
    public static string APColorToHex(this Color color)
    {
        return $"{color.R:X2}{color.G:X2}{color.B:X2}";
    }
}