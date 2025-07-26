using CornKidzAP.Archipelago;
using HarmonyLib;
using UnityEngine;

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
            if (!ArchipelagoClient.SlotData.IsOpenWollowsHollow)
                return;
            if (__instance.manualID != 104 || __instance.bOn)
                return;
            
            __instance.offLoad.m_PersistentCalls.RemoveListener(2);
        }
    }
    
    /// <summary>
    /// Always enabled the SaveTrigger for switch 401
    /// </summary>
    [HarmonyPatch(typeof(SaveTrigger), "Load")]
    public static class OpenHollow
    {
        [HarmonyPrefix]
        private static bool Prefix(SaveTrigger __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;
            if (!ArchipelagoClient.SlotData.IsOpenWollowsHollow)
                return true;
            if (__instance.id != 401 || __instance.bOn || __instance.bLoaded)
                return true;
            
            APLocationChecker.FakeLoadSaveTrigger(__instance, __instance.GetComponent<Trigger>(), true);
            GameObject.Find("SAVES/boards")?.SetActive(false);
            return false;
        }
    }
    
    // /// <summary>
    // /// Always enabled the CopySaveTrigger for switch 230 (lv1Complete)
    // /// </summary>
    // [HarmonyPatch(typeof(CopySaveTrigger), "Load")]
    // public static class StartPostOwllohMode
    // {
    //     [HarmonyPrefix]
    //     private static bool Prefix(CopySaveTrigger __instance)
    //     {
    //         if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
    //             return true;
    //         if (!ArchipelagoClient.SlotData.IsOpenMode)
    //             return true;
    //         if (__instance.manualID != 230 || __instance.bOn)
    //             return true;
    //         
    //         __instance.bOn = true;
    //         return true;
    //     }
    // }
}