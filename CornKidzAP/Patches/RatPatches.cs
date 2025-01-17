using CornKidzAP.Archipelago;
using HarmonyLib;
using UnityEngine;

namespace CornKidzAP.Patches;

public class RatPatches
{
    /// <summary>
    /// Send a location when a Rat dies
    /// </summary>
    [HarmonyPatch(typeof(Rat1), "StateDie")]
    public static class RatSender
    {
        [HarmonyPostfix]
        public static void Postfix(Rat1 __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;

            if (!ArchipelagoClient.SlotData.IsRatsanity)
                return;
            
            //just in case Rat1 is used else where
            if (GameCtrl.instance.currentWorld != 2 || GameCtrl.instance.goToDoor != 19)
                return;

            var locID = APLookup.GetAPLocationForRat(__instance);
            if (locID == null)
                return;

            if (!ArchipelagoClient.Session.Locations.AllMissingLocations.Contains(locID.Value))
                return;

            APLocationChecker.SendLocations(locID.Value).Forget();
        }
    }

    
    [HarmonyPatch(typeof(SaveTrigger), "Awake")]
    public static class SaveTriggerRemoveRatDisable
    {
        [HarmonyPostfix]
        public static void Prefix(SaveTrigger __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;
            
            if(!ArchipelagoClient.SlotData.IsRatsanity)
                return;
            
            if (__instance.gameObject.name != "arenaClear (1)" || (__instance.transform.GetComponentInParent<World>()?.worldNum ?? 0) != 2)
                return;

            var target = __instance.onLoad.m_PersistentCalls.m_Calls[17].target as GameObject;
            if (target == null || target.name != "rats")
            {
                return;
            }
            __instance.onLoad.m_PersistentCalls.RemoveListener(17); //remove rats disable
        }
    }

    [HarmonyPatch(typeof(Rat1), "Start")]
    public static class DisableRatIfCollected
    {
        [HarmonyPostfix]
        public static void Postfix(Rat1 __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;
            
            if(!ArchipelagoClient.SlotData.IsRatsanity)
                return;
            
            var locId = APLookup.GetAPLocationForRat(__instance);
            if(locId == null)
                return;
            
            if(!ArchipelagoClient.Session.Locations.AllLocationsChecked.Contains(locId.Value))
                return;
            
            __instance.gameObject.SetActive(false);
        }
    }


    [HarmonyPatch(typeof(TriggerArray), nameof(TriggerArray.CheckTrigs))]
    public static class RatsPreventVanillaTrigger
    {
        [HarmonyPrefix]
        public static bool Prefix(TriggerArray __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;
            
            if(!ArchipelagoClient.SlotData.IsRatsanity)
                return true;

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (GameCtrl.instance.currentWorld != 2 || __instance.gameObject.name != "arenaClear (1)")
                return true;
            
            return false;
        }
    }

    [HarmonyPatch(typeof(TriggerArray), "Update")]
    public static class RatsUpdateCount
    {
        [HarmonyPostfix]
        public static void Postfix(TriggerArray __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;
            
            if(!ArchipelagoClient.SlotData.IsRatsanity)
                return;

            if (GameCtrl.instance.currentWorld != 2 || __instance.gameObject.name != "arenaClear (1)")
                return;

            var rats = ArchipelagoClient.ArchipelagoData?.Rats ?? 0;
            __instance.current = rats;
            UI.instance.score = rats;
            GameCtrl.instance.data.switches[202] = rats >= 6;

            var trigger = __instance.GetComponent<Trigger>();
            var saveTrigger = __instance.GetComponent<SaveTrigger>();
            //CornKidzAP.Logger.LogDebug($"RatsUpdateCount: {rats} {trigger?.bOn}");
            if (trigger == null || saveTrigger == null)
                return;
            if (rats < 6 || trigger.bOn)
                return;

            trigger.bOn = true;
            saveTrigger.bOn = true;
            if (GameCtrl.instance.goToDoor != 19 || !saveTrigger.bLoaded) return;
            saveTrigger.onLoad.Invoke();
        }
    }
}