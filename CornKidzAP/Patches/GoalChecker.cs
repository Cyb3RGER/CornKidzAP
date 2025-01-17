using System.Collections.Generic;
using CornKidzAP.Archipelago;
using HarmonyLib;

namespace CornKidzAP.Patches;

public class GoalChecker
{
    private static readonly Dictionary<GoalTypes, int> GoalToSwitchId = new()
    {
        { GoalTypes.Owlloh, 230 }, //DefeatOwlloh switch 230?
        { GoalTypes.Tower, 328 }, //TowerComplete
        //{ GoalTypes.Anxiety, -3002 }, //AnxietyComplete
        //{ GoalTypes.God, -3003 }, //DogGod
    };


    [HarmonyPatch(typeof(SaveTrigger), "SetData")]
    public static class GoalSaveTriggerChecker
    {
        [HarmonyPostfix]
        public static void Postfix(SaveTrigger __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;

            if (ArchipelagoClient.HasBeatenGoal)
                return;
            
            if (!__instance.bOn)
                return;

            if (!GoalToSwitchId.TryGetValue(ArchipelagoClient.SlotData.Goal, out var id))
                return;

            if (__instance.id != id) return;
            ArchipelagoClient.SetGoalAchieved();
        }
    }

    [HarmonyPatch(typeof(GameCtrl), "Update")]
    public static class GoalSaveTriggerCheckerContinuous
    {
        [HarmonyPostfix]
        public static void Postfix(GameCtrl __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;

            if (ArchipelagoClient.HasBeatenGoal || GameCtrl.instance.currentWorld < 0)
                return;

            if (!GoalToSwitchId.TryGetValue(ArchipelagoClient.SlotData.Goal, out var id))
                return;

            if (!__instance.data.switches[id]) return;
            ArchipelagoClient.SetGoalAchieved();
        }
    }

    [HarmonyPatch(typeof(Results), "Start")]
    public static class GoalCheckerResults
    {
        [HarmonyPostfix]
        public static void Postfix(Results __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;
            if(ArchipelagoClient.HasBeatenGoal)
                return;
            if ((ArchipelagoClient.SlotData.Goal != GoalTypes.Anxiety || GameCtrl.instance.lastScene != "TowerN00") &&
                (ArchipelagoClient.SlotData.Goal != GoalTypes.God || GameCtrl.instance.lastScene != "secretZone00")) 
                return;
            ArchipelagoClient.SetGoalAchieved();
        }
    }
}