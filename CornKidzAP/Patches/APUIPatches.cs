using System;
using CornKidzAP.Archipelago;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CornKidzAP.Patches;

public class APUIPatches
{
    /// <summary>
    /// Show the Goal in the main menu's objective text
    /// </summary>
    [HarmonyPatch(typeof(UI), "RunMenu")]
    public static class APUIShowGoalAsObjective
    {
        [HarmonyPostfix]
        public static void Postfix(UI __instance, int ___currentMenu, int ___menuTimer, Text ___quest)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;
            if (___currentMenu != 0 || ___menuTimer != 1) return;
            if (GameCtrl.instance.currentWorld <= -100) return;
            ___quest.gameObject.SetActive(!ArchipelagoClient.HasBeatenGoal);
            ___quest.text = ArchipelagoClient.SlotData.Goal switch
            {
                GoalTypes.Owlloh => "BEAT OWLLOH",
                GoalTypes.Tower => "ESCAPE DREAM",
                GoalTypes.Anxiety => "BEAT ANXIETY TOWER",
                GoalTypes.God => "MEET DOG GOD",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
    
    /// <summary>
    /// Shows/Hides the Tracker UI when not on the main menu, for example in the options menu
    /// </summary>
    [HarmonyPatch(typeof(UI), "Update")]
    public static class APShowAndHideTracker
    {
        [HarmonyPostfix]
        public static void Postfix(UI __instance, int ___currentMenu)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;
            if (!ArchipelagoClient.APMainMenuTrackerUI)
                return;
            if(!ArchipelagoClient.APMainMenuTrackerUI.HUDObject)
                return;
            if (ArchipelagoClient.APMainMenuTrackerUI.HUDObject.activeSelf == (___currentMenu == 0)) return;
            ArchipelagoClient.APMainMenuTrackerUI.HUDObject.SetActive(___currentMenu == 0);
        }
    }

    [HarmonyPatch(typeof(UI), "Awake")]
    public static class APUIPatch
    {
        [HarmonyPostfix]
        public static void Postfix(UI __instance)
        {
            ArchipelagoClient.ConnectionInfo ??= APConnectionInfo.LoadFromDisk();
            CreateUI(__instance);
        }

        private static void CreateUI(UI __instance)
        {
            if (ArchipelagoClient.APConnectionUI) return;
            //inject all the objects/components we need for AP
            var guiGameObject = new GameObject("AP");
            ArchipelagoClient.APConnectionUI = guiGameObject.AddComponent<APConnectionUI>();
            ArchipelagoClient.APConsole = guiGameObject.AddComponent<APConsole>();
            ArchipelagoClient.APTrapHandler = guiGameObject.AddComponent<APTrapHandler>();
            ArchipelagoClient.APDeathLinkHandler = guiGameObject.AddComponent<APDeathLinkHandler>();
            if (!__instance.GetComponent<APNotificationUI>())
                ArchipelagoClient.APNotificationUI = __instance.gameObject.AddComponent<APNotificationUI>();
            if (!__instance.GetComponent<APMainMenuTrackerUI>())
                ArchipelagoClient.APMainMenuTrackerUI = __instance.gameObject.AddComponent<APMainMenuTrackerUI>();
            Object.DontDestroyOnLoad(guiGameObject);
        }
    }

    [HarmonyPatch(typeof(UI), "Start")]
    public static class APUIPatch2
    {
        [HarmonyPostfix]
        public static void Postfix(UI __instance)
        {
            if (!__instance.GetComponent<APNotificationUI>())
                ArchipelagoClient.APNotificationUI = __instance.gameObject.AddComponent<APNotificationUI>();
            if (!__instance.GetComponent<APMainMenuTrackerUI>())
                ArchipelagoClient.APMainMenuTrackerUI = __instance.gameObject.AddComponent<APMainMenuTrackerUI>();
        }
    }
}