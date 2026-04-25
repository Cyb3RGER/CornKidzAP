using System;
using CornKidzAP.Archipelago;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CornKidzAP.Patches;

public class TestZoneCubePatches
{
    [HarmonyPatch(typeof(TriggerArray), "Update")]
    public class ContinuousTriggerArrayChecker
    {
        [HarmonyPrefix]
        public static bool Prefix(TriggerArray __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;

            if (!ArchipelagoClient.SlotData.IsTestCubesanity)
                return true;

            if (__instance.name != "cubeappear" || SceneManager.GetActiveScene().name != "TestZoneX")
                return true;

            if (ArchipelagoClient.ArchipelagoData.TestZoneCube < 25)
                return false;

            var trigger = __instance.GetComponent<Trigger>();
            if (trigger.bOn)
                return false;

            trigger.Activate();
            trigger.bOn = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(Trigger), "Activate")]
    public class DisableTriggerActivate
    {
        [HarmonyPrefix]
        public static bool Prefix(Trigger __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;

            if (!ArchipelagoClient.SlotData.IsTestCubesanity)
                return true;

            if (__instance.name != "cubeappear" || SceneManager.GetActiveScene().name != "TestZoneX")
                return true;

            if (ArchipelagoClient.ArchipelagoData.TestZoneCube < 25)
                return false;

            return true;
        }
    }

    [HarmonyPatch(typeof(Trigger), "Activate")]
    public class CubeLocationSender
    {
        [HarmonyPrefix]
        public static bool Prefix(Trigger __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;

            if (!ArchipelagoClient.SlotData.IsTestCubesanity)
                return true;

            if (!__instance.name.StartsWith("item ") || SceneManager.GetActiveScene().name != "TestZoneX")
                return true;

            var index = -1;
            if (__instance.transform.parent?.name == "items")
            {
                index = __instance.transform.GetSiblingIndex() + 1;
            }

            if (__instance.transform.parent?.name == "chestItem")
            {
                index = 25;
            }

            if (index < 0)
            {
                CornKidzAP.Logger.LogWarning($"Invalid sibling index for cube trigger {__instance.name} {index}");
                return true;
            }

            var locId = APLookup.GetAPLocationForTestCubeIndex(index);
            if (locId == null)
                return true;

            if (!ArchipelagoClient.Session.Locations.AllMissingLocations.Contains(locId.Value))
                return true;

            APLocationChecker.SendLocations(locId.Value).Forget();
            return true;
        }
    }

    [HarmonyPatch(typeof(Trigger), "Start")]
    public class CubeLocationCollector
    {
        [HarmonyPrefix]
        public static bool Prefix(Trigger __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;

            if (!ArchipelagoClient.SlotData.IsTestCubesanity)
                return true;

            if (!__instance.name.StartsWith("item ") || SceneManager.GetActiveScene().name != "TestZoneX")
                return true;

            var index = -1;
            if (__instance.transform.parent?.name == "items")
            {
                index = __instance.transform.GetSiblingIndex() + 1;
            }

            if (__instance.transform.parent?.name == "chestItem")
            {
                index = 25;
            }

            if (index < 0)
            {
                CornKidzAP.Logger.LogWarning($"Invalid sibling index for cube trigger {__instance.name} {index}");
                return true;
            }

            var locId = APLookup.GetAPLocationForTestCubeIndex(index);
            if (locId == null)
                return true;

            if (!ArchipelagoClient.Session.Locations.AllLocationsChecked.Contains(locId.Value))
                return false;

            __instance.Activate(); //tODO: I think this does the pickup effect which is not needed here
            return false;
        }
    }

    [HarmonyPatch(typeof(Trigger), "Update")]
    public class CubeLocationCollectorContinuous
    {
        [HarmonyPrefix]
        public static bool Prefix(Trigger __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;

            if (!ArchipelagoClient.SlotData.IsTestCubesanity)
                return true;

            if (!__instance.name.StartsWith("item ") || SceneManager.GetActiveScene().name != "TestZoneX")
                return true;

            var index = -1;
            if (__instance.transform.parent?.name == "items")
            {
                index = __instance.transform.GetSiblingIndex() + 1;
            }

            if (__instance.transform.parent?.name == "chestItem")
            {
                index = 25;
            }

            if (index < 0)
            {
                CornKidzAP.Logger.LogWarning($"Invalid sibling index for cube trigger {__instance.name} {index}");
                return true;
            }

            var locId = APLookup.GetAPLocationForTestCubeIndex(index);
            if (locId == null)
                return true;

            if (!ArchipelagoClient.Session.Locations.AllLocationsChecked.Contains(locId.Value))
                return false;

            if (!__instance.bOn)
                __instance.Activate(); //tODO: I think this does the pickup effect which is not needed here

            return false;
        }
    }
}