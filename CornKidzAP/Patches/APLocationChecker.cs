using System;
using CornKidzAP.Archipelago;
using HarmonyLib;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace CornKidzAP.Patches;


public class APLocationChecker
{
    /// <summary>
    /// Fakes collecting the item by hiding/disabling it but not setting the ingame flag
    /// </summary>
    /// <param name="__instance"></param>
    /// <param name="isPlayerInitated"></param>
    /// <param name="forceHide"></param>
    public static void FakeCollectSaveItem(SaveItem __instance, bool isPlayerInitated, bool forceHide)
    {
        __instance.bCollected = true;
        if (forceHide || !__instance.bNoHide)
        {
            foreach (var renderer in __instance.transform.GetComponentsInChildren<Renderer>())
                renderer.gameObject.SetActive(false);
        }

        var collider = __instance.GetComponent<Collider>();
        if (collider)
            collider.enabled = false;

        if (isPlayerInitated)
        {
            __instance.GetComponent<Trigger>()?.Activate();
        }
        else
        {
            __instance.GetComponent<Trigger>()?.Load(__instance.bCollected);
        }
    }

    public static void FakeCollectUpgradeItem(UpgradeItem __instance)
    {
        __instance.bCollected = true;
        foreach (var renderer in __instance.transform.GetComponentsInChildren<Renderer>())
            renderer.gameObject.SetActive(false);
        var collider = __instance.GetComponent<Collider>();
        if (collider)
            collider.enabled = false;
    }
    
    public static void FakeLoadCopySaveTrigger(CopySaveTrigger instance, Trigger trigger, bool on)
    {
        instance.bOn = on;
        instance.bLoaded = true;
        if (instance.bOn)
        {
            instance.GetComponent<Break>()?.Hide();
            instance.onLoad.Invoke();
        }
        else
        {
            instance.offLoad.Invoke();
        }

        trigger?.Load(instance.bOn);
    }

    public static void FakeLoadSaveTrigger(SaveTrigger instance, Trigger trigger, bool on)
    {
        instance.bOn = on;
        instance.bLoaded = true;
        if (instance.bOn)
            instance.onLoad.Invoke();
        else
            instance.offLoad.Invoke();
        trigger?.Load(instance.bOn);
    }

    public static async UniTaskVoid SendLocations(params long[] ids)
    {
        try
        {
            await ArchipelagoClient.Session.Locations.CompleteLocationChecksAsync(ids);
        }
        catch (Exception e)
        {
            CornKidzAP.Logger.LogError($"Error sending location(s) {ids}:{Environment.NewLine}{e}");
        }
    }

    /// <summary>
    /// Patch to send out Achievements
    /// </summary>
    [HarmonyPatch(typeof(GameCtrl), nameof(GameCtrl.UnlockAchievement))]
    public static class AchievementSender
    {
        [HarmonyPrefix]
        public static void Prefix(GameCtrl __instance, int id)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;

            if (!ArchipelagoClient.SlotData.IsAchievementsanity)
                return;

            CornKidzAP.Logger.LogDebug($"Trying to unlock achievement {id}");
            var locId = APLookup.GetAPLocationForAchievementId(id);
            if (locId == null)
                return;

            if (!ArchipelagoClient.Session.Locations.AllMissingLocations.Contains(locId.Value))
                return;

            SendLocations(locId.Value).Forget();
        }
    }

    /// <summary>
    /// Send Text-based locations
    /// </summary>
    [HarmonyPatch(typeof(UI), nameof(UI.ShowText))]
    public static class DialogSender
    {
        [HarmonyPostfix]
        public static void Postfix(UI __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;

            if (__instance.textLine.Length < 1)
                return;
            var locId = APLookup.GetAPLocationForString(__instance.textLine[0].textys);
            if (locId == null)
                return;

            if (!ArchipelagoClient.Session.Locations.AllMissingLocations.Contains(locId.Value))
                return;

            SendLocations(locId.Value).Forget();
        }
    }
}