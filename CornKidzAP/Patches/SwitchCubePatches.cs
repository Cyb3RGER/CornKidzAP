using CornKidzAP.Archipelago;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CornKidzAP.Patches;

public class SwitchCubePatches
{
    /// <summary>
    /// Patch to update the doorlights (CopySaveTrigger) based on AP state
    /// </summary>
    [HarmonyPatch(typeof(CopySaveTrigger), "Update")]
    public static class SwitchCubeCollectorDoorlights
    {
        [HarmonyPostfix]
        public static bool Prefix(CopySaveTrigger __instance, Trigger ___trigger)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;
            if (!ArchipelagoClient.SlotData.IsSwitchsanity)
                return true;
            if ((!__instance.saveTrigger || __instance.saveTrigger.id is < 512 or > 514) && __instance.manualID != 515)
                return true;

            
            var id = __instance.saveTrigger ? __instance.saveTrigger.id : __instance.manualID;
            var locID = APLookup.GetAPLocationForSwitchId(id);
            if (locID is null)
                return true;

            if (512 - id + ArchipelagoClient.ArchipelagoData.SomeOtherPlaceSwitches <= 0)
                return false;
            
            APLocationChecker.FakeLoadCopySaveTrigger(__instance, ___trigger, true);
            
            return false;
        }
    }

    /// <summary>
    /// Patch to update the switches (SaveTrigger) based on AP state
    /// </summary>
    [HarmonyPatch(typeof(SaveTrigger), "Update")]
    public static class SwitchCubeCollector
    {
        [HarmonyPrefix]
        public static bool Prefix(SaveTrigger __instance, Trigger ___trigger)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;
            if (!ArchipelagoClient.SlotData.IsSwitchsanity)
                return true;
            if (__instance.bOn || __instance.id is < 512 or > 515)
                return true;

            var locID = APLookup.GetAPLocationForSwitchId(__instance.id);
            if (locID is null)
                return true;
            
            if (!ArchipelagoClient.Session.Locations.AllLocationsChecked.Contains(locID.Value))
                return false;
            
            APLocationChecker.FakeLoadSaveTrigger(__instance, ___trigger, true);
            return false;
        }
    }

    /// <summary>
    /// Patch for switch to send their AP location when hit
    /// </summary>
    [HarmonyPatch(typeof(SaveTrigger), "Update")]
    public static class SwitchCubeSender
    {
        [HarmonyPrefix]
        public static bool Prefix(SaveTrigger __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;
            if (!ArchipelagoClient.SlotData.IsSwitchsanity)
                return true;
            if (!__instance.bOn || __instance.id is < 512 or > 515)
                return true;

            var locId = APLookup.GetAPLocationForSwitchId(__instance.id);
            if (locId is null)
                return true;

            if (!ArchipelagoClient.Session.Locations.AllMissingLocations.Contains(locId.Value))
                return false;

            APLocationChecker.SendLocations(locId.Value).Forget();
            return false;
        }
    }
    
    [HarmonyPatch(typeof(TriggerArray), "Update")]
    public class DoorChecker
    {
        [HarmonyPrefix]
        public static bool Prefix(TriggerArray __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;

            if (!ArchipelagoClient.SlotData.IsTestCubesanity)
                return true;

            if (__instance.name != "array" || SceneManager.GetActiveScene().name != "secretZone00")
                return true;

            if (ArchipelagoClient.ArchipelagoData.SomeOtherPlaceSwitches < 4)
                return false;

            var trigger = __instance.GetComponent<Trigger>();
            if (trigger.bOn)
                return false;

            trigger.Activate();
            trigger.bOn = true;
            return false;
        }
    }
}