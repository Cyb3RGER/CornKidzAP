using Archipelago.MultiClient.Net.Models;

namespace CornKidzAP.Archipelago;

public static class Utils
{
    public static string APColorToHex(this Color color)
    {
        return $"{color.R:X2}{color.G:X2}{color.B:X2}";
    }
    
    public struct CopySettings
    {
        public bool bVsync;
        public bool bRAnalogDpad;
        public bool bAnalogX2;
        public bool bIgnoreDpadX;
        public bool bP1only;
        public bool bAchDisable;
        public bool bForce60hz;
    }

    public static CopySettings GetCopySettings(this Data data)
    {
        return new CopySettings
        {
            bVsync = data.bVsync,
            bRAnalogDpad = data.bRAnalogDpad,
            bAnalogX2 = data.bAnalogX2,
            bIgnoreDpadX = data.bIgnoreDpadX,
            bP1only = data.bP1only,
            bAchDisable = data.bAchDisable,
            bForce60hz = data.bForce60hz
        };
    }
    
    public static void ApplyCopySettings(this Data data, CopySettings copySettings)
    {
        data.bVsync = copySettings.bVsync;
        data.bRAnalogDpad = copySettings.bRAnalogDpad;
        data.bAnalogX2 = copySettings.bAnalogX2;
        data.bIgnoreDpadX = copySettings.bIgnoreDpadX;
        data.bP1only = copySettings.bP1only;
        data.bAchDisable = copySettings.bAchDisable;
        data.bForce60hz = copySettings.bForce60hz;
    }
}