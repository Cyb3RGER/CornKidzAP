using CornKidzAP.Archipelago;
using HarmonyLib;

namespace CornKidzAP.Patches;

public class FishPatches
{
    /// <summary>
    /// Patch for the fish icons to load properly based on AP-State instead of in-game flag
    /// </summary>
    [HarmonyPatch(typeof(Fish1), "StateLoop")]
    public static class FishSender
    {
        [HarmonyPostfix]
        public static void Postfix(Fish1 __instance)
        {
            if (__instance.badState != Bad.State.die)
                return;
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;
            if (!ArchipelagoClient.SlotData.IsFishsanity)
                return;
            var copySaveTrigger = __instance.GetComponent<CopySaveTrigger>();
            if (!copySaveTrigger || copySaveTrigger.saveTrigger is null)
                return;
            var locId = APLookup.GetAPLocationForSwitchId(copySaveTrigger.saveTrigger.id);
            if (locId is null)
                return;
            if (!ArchipelagoClient.Session.Locations.AllMissingLocations.Contains(locId.Value))
                return;
            APLocationChecker.SendLocations(locId.Value).Forget();
        }
    }
    
    /// <summary>
    /// Patch for the fish icons to load properly based on AP-State instead of in-game flag
    /// </summary>
    [HarmonyPatch(typeof(SaveTrigger), nameof(SaveTrigger.Load))]
    public static class FishIconTriggerLoader
    {
        [HarmonyPrefix]
        public static bool Prefix(SaveTrigger __instance, Trigger ___trigger)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;
            if (!ArchipelagoClient.SlotData.IsFishsanity)
                return true;
            if(__instance.id is < 238 or > 240)
                return true;
            
            APLocationChecker.FakeLoadSaveTrigger(__instance, ___trigger, 238 - __instance.id + ArchipelagoClient.ArchipelagoData.Fish > 0);
            return false;
        }
    }
    
    
    /// <summary>
    /// Patch for the fish icons to trigger when getting a fish through AP
    /// </summary>
    [HarmonyPatch(typeof(SaveTrigger), "Update")]
    public static class FishIconTriggerCollector
    {
        [HarmonyPrefix]
        public static bool Prefix(SaveTrigger __instance, Trigger ___trigger)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;
            if (!ArchipelagoClient.SlotData.IsFishsanity)
                return true;
            if(__instance.bOn || __instance.id is < 238 or > 240)
                return true;

            if (238 - __instance.id + ArchipelagoClient.ArchipelagoData.Fish <= 0) 
                return true;
            APLocationChecker.FakeLoadSaveTrigger(__instance, ___trigger, true);
            return false;
        }
    }
    
    /// <summary>
    /// Patch to load the Fish based on AP State instead of in-game flag
    /// </summary>
    [HarmonyPatch(typeof(CopySaveTrigger), "Load")]
    public static class FishLoader
    {
        [HarmonyPrefix]
        public static bool Prefix(CopySaveTrigger __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;
            if (!ArchipelagoClient.SlotData.IsFishsanity)
                return true;
            if (!__instance.saveTrigger || __instance.saveTrigger.id is < 238 or > 240)
                return true;

            var locID = APLookup.GetAPLocationForSwitchId(__instance.saveTrigger.id);
            if (locID is null)
                return true;
            
            APLocationChecker.FakeLoadCopySaveTrigger(__instance, null, ArchipelagoClient.Session.Locations.AllLocationsChecked.Contains(locID.Value));
            return false;
        }
    }
    
    /// <summary>
    ///  Patch to update the Fish based on AP State instead of in-game flag
    /// </summary>
    [HarmonyPatch(typeof(CopySaveTrigger), "Update")]
    public static class FishCollectorContinuous
    {
        [HarmonyPostfix]
        public static void Postfix(CopySaveTrigger __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;
            if (!ArchipelagoClient.SlotData.IsFishsanity)
                return;
            if (__instance.bOn || !__instance.saveTrigger || __instance.saveTrigger.id is < 238 or > 240)
                return;

            var locID = APLookup.GetAPLocationForSwitchId(__instance.saveTrigger.id);
            if (locID is null)
                return;

            if (!ArchipelagoClient.Session.Locations.AllLocationsChecked.Contains(locID.Value))
                return;
            APLocationChecker.FakeLoadCopySaveTrigger(__instance, null, true);
        }
    }
}