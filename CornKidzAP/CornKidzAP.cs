using BepInEx;
using BepInEx.Logging;
using CornKidzAP.Archipelago;
using HarmonyLib;

namespace CornKidzAP;

[BepInPlugin(PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Corn Kidz 64.exe")]
public class CornKidzAP : BaseUnityPlugin
{
    private const string PLUGIN_GUID = "gg.archipelago.CornKidzAP";
    internal new static ManualLogSource Logger;
    private Harmony _harmony;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");
        _harmony = new Harmony(PLUGIN_GUID);
        _harmony.PatchAll();
        Logger.LogInfo("Harmony patches applied!");
    }

    private void Update()
    {
        ArchipelagoClient.TryGiveEnqueuedItems();
    }
}
