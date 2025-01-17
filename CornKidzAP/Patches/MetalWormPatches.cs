using CornKidzAP.Archipelago;
using HarmonyLib;

namespace CornKidzAP.Patches;

public class MetalWormPatches
{
    /// <summary>
    /// Send the metal worm
    /// </summary>
    [HarmonyPatch(typeof(SaveTrigger), nameof(SaveTrigger.SetData))]
    public static class MetalWormSender
    {
        [HarmonyPostfix]
        public static void Postfix(SaveTrigger __instance, bool bSet)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;

            if (__instance.id != 236)
                return;

            var locId = APLookup.GetAPLocationForSwitchId(__instance.id);
            if (locId == null)
                return;
            
            if (!ArchipelagoClient.Session.Locations.AllMissingLocations.Contains(locId.Value))
                return;
            
            __instance.bOn = bSet;
            if (!__instance.bOn)
                return;
            APLocationChecker.SendLocations(locId.Value).Forget();
        }
    }
    
    /// <summary>
    /// Prevent the metal worm de-spawning based on the in-game switch (SaveTrigger) flag
    /// </summary>
    [HarmonyPatch(typeof(SaveTrigger), "Load")]
    public static class MetalWormLoader
    {
        [HarmonyPrefix]
        public static bool Prefix(SaveTrigger __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;

            if (__instance.id != 236)
                return true;

            var locId = APLookup.GetAPLocationForSwitchId(__instance.id);
            if (locId == null)
                return true;

            __instance.bOn = ArchipelagoClient.Session.Locations.AllLocationsChecked.Contains(locId.Value);
            if (__instance.bOn)
            {
                //fake collect
                __instance.transform.parent.gameObject.SetActive(false);
            }

            __instance.bLoaded = true;
            return false;
        }
    }
    
    /// <summary>
    /// Prevent the metal worm de-spawning based on the in-game switch (SaveTrigger) flag
    /// </summary>
    [HarmonyPatch(typeof(SaveTrigger), "Update")]
    public static class MetalWormCollector
    {
        [HarmonyPostfix]
        public static void Postfix(SaveTrigger __instance)
        {
            if (__instance.bOn)
                return;
            
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;

            if (__instance.id != 236)
                return;

            var locId = APLookup.GetAPLocationForSwitchId(__instance.id);
            if (locId == null)
                return;

            if(!ArchipelagoClient.Session.Locations.AllLocationsChecked.Contains(locId.Value))
                return;
            
            __instance.bOn = true;
            __instance.transform.parent.gameObject.SetActive(false);
            __instance.bLoaded = true;
        }
    }

    /// <summary>
    /// Patch to trigger CopySaveTrigger to destroy pellet 
    /// </summary>
    [HarmonyPatch(typeof(CopySaveTrigger), "Load")]
    public static class MetalWormCopyLoader
    {
        [HarmonyPrefix]
        public static bool Prefix(CopySaveTrigger __instance, Trigger ___trigger)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;

            // skip the zoo pig
            if (!__instance.saveTrigger || __instance.saveTrigger.id != 236 || __instance.name != "pellet.002 (2)")
                return true;

            var locId = APLookup.GetAPLocationForSwitchId(__instance.saveTrigger.id);
            if (locId == null)
                return true;

            APLocationChecker.FakeLoadCopySaveTrigger(__instance, ___trigger, ArchipelagoClient.Session.Locations.AllLocationsChecked.Contains(locId.Value));
            return false;
        }
    }

    /// <summary>
    /// Patch to trigger CopySaveTrigger for the metal worm when collected (continuous)
    /// </summary>
    [HarmonyPatch(typeof(CopySaveTrigger), "Update")]
    public static class MetalWormCopyCollector
    {
        [HarmonyPostfix]
        public static void Postfix(CopySaveTrigger __instance, Trigger ___trigger)
        {
            if (__instance.bOn)
                return;
            
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;

            if (!__instance.saveTrigger || __instance.saveTrigger.id != 236 || __instance.name != "pellet.002 (2)")
                return;

            var locId = APLookup.GetAPLocationForSwitchId(__instance.saveTrigger.id);
            if (locId == null)
                return;

            if(!ArchipelagoClient.Session.Locations.AllLocationsChecked.Contains(locId.Value))
                return;
            
            APLocationChecker.FakeLoadCopySaveTrigger(__instance, ___trigger, true);
        }
    }
}