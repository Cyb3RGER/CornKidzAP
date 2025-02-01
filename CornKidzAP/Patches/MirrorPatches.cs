using System.Collections.Generic;
using CornKidzAP.Archipelago;
using HarmonyLib;

namespace CornKidzAP.Patches;

public class MirrorPatches
{
    private static readonly List<int> MirrorSwitchIds = [141, 142, 284, 288, 289, 290];
    
    /// <summary>
    /// Patch to remove mirror that have already been collected
    /// </summary>
    [HarmonyPatch(typeof(CopySaveTrigger), "Update")]
    public static class MirrorCollector
    {
        [HarmonyPostfix]
        public static void Postfix(CopySaveTrigger __instance)
        {
            if (__instance.bOn)
                return;

            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;
            
            if (!MirrorSwitchIds.Contains(__instance.saveItem?.id ?? 0) && !MirrorSwitchIds.Contains(__instance.manualIDitem))
                return;

            var locId = APLookup.GetAPLocationForSaveItemId(__instance.saveItem?.id ?? __instance.manualIDitem);
            if (locId == null)
                return;

            if (!ArchipelagoClient.Session.Locations.AllLocationsChecked.Contains(locId.Value))
                return;
            
            APLocationChecker.FakeLoadCopySaveTrigger(__instance, null, true);
        }
    }
}