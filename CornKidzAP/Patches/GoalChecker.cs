using System.Collections.Generic;
using CornKidzAP.Archipelago;
using HarmonyLib;

namespace CornKidzAP.Patches;

public class GoalChecker
{
    private static readonly Dictionary<int, GoalSelection> SwitchIdToGoal = new()
    {
        { 230, GoalSelection.Owlloh }, //DefeatOwlloh switch 230?
        { 328, GoalSelection.Tower }, //TowerComplete
    };
    
    private static readonly Dictionary<string, GoalSelection> SceneIdToGoal = new()
    {
        {"TowerN00", GoalSelection.Anxiety}, //Anxiety Tower
        {"secretZone00", GoalSelection.God} //Dog God
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

            if (!SwitchIdToGoal.TryGetValue(__instance.id, out var possibleGoal))
                return;

            if (!ArchipelagoClient.SlotData.Goals.HasFlag(possibleGoal) || ArchipelagoClient.ArchipelagoData.BeatenGoals.HasFlag(possibleGoal))
                return;
            
            ArchipelagoClient.ArchipelagoData.BeatenGoals |= possibleGoal;
            
            if(ArchipelagoClient.ArchipelagoData.BeatenGoals == ArchipelagoClient.SlotData.Goals)
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

            foreach (var (id, goal) in SwitchIdToGoal)
            {
                if(!__instance.data.switches[id])
                    continue;
                
                if (!ArchipelagoClient.SlotData.Goals.HasFlag(goal) || ArchipelagoClient.ArchipelagoData.BeatenGoals.HasFlag(goal))
                    continue;
                
                ArchipelagoClient.ArchipelagoData.BeatenGoals |= goal;

            }
            
            if(ArchipelagoClient.ArchipelagoData.BeatenGoals == ArchipelagoClient.SlotData.Goals)
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
            if (ArchipelagoClient.HasBeatenGoal)
                return;

            if(!SceneIdToGoal.TryGetValue(GameCtrl.instance.lastScene,out var possibleGoal))
                return;
            
            if (!ArchipelagoClient.SlotData.Goals.HasFlag(possibleGoal) || ArchipelagoClient.ArchipelagoData.BeatenGoals.HasFlag(possibleGoal))
                return;
            
            ArchipelagoClient.ArchipelagoData.BeatenGoals |= possibleGoal;
            
            if(ArchipelagoClient.ArchipelagoData.BeatenGoals == ArchipelagoClient.SlotData.Goals)
                ArchipelagoClient.SetGoalAchieved();
        }
    }
}