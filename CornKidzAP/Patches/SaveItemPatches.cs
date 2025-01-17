using CornKidzAP.Archipelago;
using HarmonyLib;

namespace CornKidzAP.Patches;


public class SaveItemPatches
{
    /// <summary>
    /// Patch to send Locations out when collecting a SaveItem
    /// </summary>
    [HarmonyPatch(typeof(SaveItem), nameof(SaveItem.Collect))]
    public static class SaveItemSender
    {
        [HarmonyPrefix]
        public static bool Prefix(SaveItem __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;

            if (__instance.bCrank && !ArchipelagoClient.SlotData.IsCranksanity)
                return true;

            var locId = APLookup.GetAPLocationForSaveItem(__instance);
            if (locId == null)
                return true;

            APLocationChecker.FakeCollectSaveItem(__instance, true, false);


            if (!ArchipelagoClient.Session.Locations.AllMissingLocations.Contains(locId.Value))
                return false;

            APLocationChecker.SendLocations(locId.Value).Forget();
            return false;
        }
    }

    /// <summary>
    /// Patch to "remove" already collected SaveItems
    /// </summary>
    [HarmonyPatch(typeof(SaveItem), "Update")]
    public static class SaveItemCollector
    {
        [HarmonyPostfix]
        public static void Postfix(SaveItem __instance)
        {
            if (__instance.bCollected)
                return;

            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;

            if (__instance.bCrank && !ArchipelagoClient.SlotData.IsCranksanity)
                return;

            var locId = APLookup.GetAPLocationForSaveItem(__instance);
            if (locId == null)
                return;

            if (!ArchipelagoClient.Session.Locations.AllLocationsChecked.Contains(locId.Value))
                return;
            
            APLocationChecker.FakeCollectSaveItem(__instance, false, true);
        }
    }

    /// <summary>
    /// Patch to "remove" already collected SaveItems
    /// </summary>
    [HarmonyPatch(typeof(SaveItem), "Load")]
    public static class SaveItemLoader
    {
        [HarmonyPrefix]
        public static bool Prefix(SaveItem __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;

            if (__instance.bCrank && !ArchipelagoClient.SlotData.IsCranksanity)
                return true;

            var locId = APLookup.GetAPLocationForSaveItem(__instance);
            if (locId == null)
                return true;

            __instance.bCollected = ArchipelagoClient.Session.Locations.AllLocationsChecked.Contains(locId.Value);
            if (__instance.bCollected)
                APLocationChecker.FakeCollectSaveItem(__instance, false, true);
            else if (__instance.bHidden)
                __instance.gameObject.SetActive(false);
            __instance.bLoaded = true;
            return false;
        }
    }
    
    /// <summary>
    /// Patch to trigger CopySaveTrigger based on AP collection state instead of in-game saveitem flag
    /// </summary>
    [HarmonyPatch(typeof(CopySaveTrigger), "Load")]
    public static class CopySaveTriggerLoader
    {
        [HarmonyPostfix]
        public static bool Prefix(CopySaveTrigger __instance, Trigger ___trigger)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;

            //skip cheese grater, we actually use that saveitem flag to track the item 
            if (__instance.saveItem == null || __instance.saveItem.id <= 0 || __instance.saveItem.id == 310)
                return true;

            var locId = APLookup.GetAPLocationForSaveItem(__instance.saveItem);
            if (locId == null)
                return true;
            
            APLocationChecker.FakeLoadCopySaveTrigger(__instance, ___trigger, ArchipelagoClient.Session.Locations.AllLocationsChecked.Contains(locId.Value));
            return false;
        }
    }
}