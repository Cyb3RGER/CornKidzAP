using System;
using CornKidzAP.Archipelago;
using HarmonyLib;
using UnityEngine.SceneManagement;

namespace CornKidzAP.Patches;

public class GameCtrlPatches
{
    [HarmonyPatch(typeof(GameCtrl), nameof(GameCtrl.SaveGame))]
    public static class SaveGamePatch
    {
        [HarmonyPostfix]
        private static bool Prefix(GameCtrl __instance)
        {
            if (!ArchipelagoClient.Authenticated)
            {
                return true;
            }
            ArchipelagoClient.ArchipelagoData.LastScene = SceneManager.GetActiveScene().name;
            try
            {
                ArchipelagoClient.ArchipelagoData.SaveToDisk();
                return false; 
            }
            catch (Exception ex)
            {
                CornKidzAP.Logger.LogError($"Failed to save game: {ex.Message}");
                return false;
            }
        }
    }
    [HarmonyPatch(typeof(GameCtrl), nameof(GameCtrl.LoadGame))]
    public static class LoadGamePatch
    {
        [HarmonyPostfix]
        private static bool Prefix(GameCtrl __instance)
        {
            if (!ArchipelagoClient.Authenticated)
            {
                return true;
            }
            try
            {
                CornKidzAP.Logger.LogDebug("GameCtrl.LoadGame");
                ArchipelagoClient.ArchipelagoData ??= ArchipelagoData.LoadFromDisk();
                GameCtrl.instance.data = ArchipelagoClient.ArchipelagoData.GameData;
                //GameCtrl.instance.data.switches[104] = true; // tutorial complete -> full moveset on start
                return false;
            }
            catch (Exception ex)
            {
                CornKidzAP.Logger.LogError($"Failed to load game: {ex.Message}");
                return false;
            }
        }
    }
}