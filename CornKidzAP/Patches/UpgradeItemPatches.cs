using CornKidzAP.Archipelago;
using HarmonyLib;

namespace CornKidzAP.Patches;

public class UpgradeItemPatches
{
    /// <summary>
    /// Patch to send out Locations when collecting an upgrade item
    /// </summary>
    [HarmonyPatch(typeof(UpgradeItem), nameof(UpgradeItem.Collect))]
    public static class UpgradeItemSender
    {
        [HarmonyPrefix]
        public static bool Prefix(UpgradeItem __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;

            var locId = APLookup.GetAPLocationForUpgrade(__instance);
            if (locId == null)
                return true;

            APLocationChecker.FakeCollectUpgradeItem(__instance);

            if (!ArchipelagoClient.Session.Locations.AllMissingLocations.Contains(locId.Value))
                return false;

            APLocationChecker.SendLocations(locId.Value).Forget();
            return false;
        }
    }

    /// <summary>
    /// Patch to "remove" already collected UpgradeItems
    /// </summary>
    [HarmonyPatch(typeof(UpgradeItem), "Load")]
    public static class UpgradeItemLoader
    {
        [HarmonyPrefix]
        public static bool Prefix(UpgradeItem __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;

            var locId = APLookup.GetAPLocationForUpgrade(__instance);
            if (locId == null)
                return true;

            __instance.bCollected = ArchipelagoClient.Session.Locations.AllLocationsChecked.Contains(locId.Value);
            if (__instance.bCollected)
                APLocationChecker.FakeCollectUpgradeItem(__instance);
            __instance.bLoaded = true;
            return false;
        }
    }

    /// <summary>
    /// Patch to "remove" already collected UpgradeItems
    /// </summary>
    [HarmonyPatch(typeof(UpgradeItem), "Update")]
    public static class UpgradeItemCollector
    {
        [HarmonyPostfix]
        public static void Postfix(UpgradeItem __instance)
        {
            if (__instance.bCollected)
                return;

            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;

            var locId = APLookup.GetAPLocationForUpgrade(__instance);
            if (locId == null)
                return;

            if (!ArchipelagoClient.Session.Locations.AllLocationsChecked.Contains(locId.Value))
                return;

            APLocationChecker.FakeCollectUpgradeItem(__instance);
        }
    }
}