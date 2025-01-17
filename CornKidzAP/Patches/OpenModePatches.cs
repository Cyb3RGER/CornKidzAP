using CornKidzAP.Archipelago;
using HarmonyLib;

namespace CornKidzAP.Patches;

public class OpenModePatches
{
    /// <summary>
    /// Removes the AttackDisable call on the CopySaveTrigger for switch 104
    /// </summary>
    [HarmonyPatch(typeof(CopySaveTrigger), "Awake")]
    public static class StartWithFullMoveset
    {
        [HarmonyPostfix]
        private static void Postfix(CopySaveTrigger __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;
            if (!ArchipelagoClient.SlotData.IsOpenMode)
                return;
            if (__instance.manualID != 104 || __instance.bOn)
                return;
            
            __instance.offLoad.m_PersistentCalls.RemoveListener(2);
        }
    }
}