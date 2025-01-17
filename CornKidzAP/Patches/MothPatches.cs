using System.Reflection;
using CornKidzAP.Archipelago;
using HarmonyLib;
using UnityEngine;

namespace CornKidzAP.Patches;

public class MothPatches
{
    /// <summary>
    /// Patch to remove moth disguise objects for moths that have already been collected
    /// </summary>
    [HarmonyPatch(typeof(CopySaveTrigger), "Update")]
    public static class MothCollector
    {
        [HarmonyPostfix]
        public static void Postfix(CopySaveTrigger __instance)
        {
            if (__instance.bOn)
                return;

            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;

            if (__instance.saveItem == null || __instance.saveItem.id <= 0)
                return;

            var locId = APLookup.GetAPLocationForSaveItem(__instance.saveItem);
            if (locId == null)
                return;

            if (!ArchipelagoClient.Session.Locations.AllLocationsChecked.Contains(locId.Value))
                return;

            var mothDisguise = __instance.gameObject.GetComponent<MothDisguise>();
            if (mothDisguise == null)
                return;

            //hide moth if already spawned
            var moth = typeof(MothDisguise).GetField("moth", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(mothDisguise) as GameObject;
            if (moth != null) moth.SetActive(false);
            APLocationChecker.FakeLoadCopySaveTrigger(__instance, null, true);
        }
    }
}