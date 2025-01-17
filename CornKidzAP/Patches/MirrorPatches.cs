using System;
using CornKidzAP.Archipelago;
using HarmonyLib;

namespace CornKidzAP.Patches;

public class MirrorPatches
{
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

            // FixMe: this check kinda sucks
            if (!__instance.transform.parent || !__instance.transform.parent.name.Contains("mirror", StringComparison.InvariantCultureIgnoreCase))
                return;
            
            if (__instance.saveItem == null || __instance.saveItem.id <= 0)
                return;

            var locId = APLookup.GetAPLocationForSaveItem(__instance.saveItem);
            if (locId == null)
                return;

            if (!ArchipelagoClient.Session.Locations.AllLocationsChecked.Contains(locId.Value))
                return;
            
            APLocationChecker.FakeLoadCopySaveTrigger(__instance, null, true);
        }
    }
}