using CornKidzAP.Archipelago;
using HarmonyLib;

namespace CornKidzAP.Patches;

public class CrankInstallPatches
{
    /// <summary>
    /// Update Drill Crank to "assembly" if we received it but haven't triggered on load
    /// </summary>
    [HarmonyPatch(typeof(CrankInstall), "Update")]
    public static class CrankCollector
    {
        [HarmonyPostfix]
        public static void Postfix(CrankInstall __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;

            if (!ArchipelagoClient.SlotData.IsCranksanity)
                return;

            var saveTrigger = __instance.GetComponent<SaveTrigger>();
            if (!saveTrigger)
                return;

            var itemId = APLookup.GetAPItemIdForCrank(saveTrigger.id);
            if (itemId == null)
                return;

            if (GameCtrl.instance.data.switches[saveTrigger.id] && !saveTrigger.bOn && saveTrigger.bLoaded)
                saveTrigger.onLoad.Invoke();
        }
    }
}