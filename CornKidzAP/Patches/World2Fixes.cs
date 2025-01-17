using HarmonyLib;

namespace CornKidzAP.Patches;

public class World2Fixes
{
    /// <summary>
    /// Disables the drop gate in Drill Tower to prevent soft-locking
    /// </summary>
    [HarmonyPatch(typeof(Trigger), "Awake")]
    public static class DisableDrillTowerGateTrigger
    {
        [HarmonyPostfix]
        public static void Postfix(Trigger __instance)
        {
            if (__instance.name != "dropgate") return;
            __instance.gameObject.SetActive(false);
        }
    }
    
    // /// <summary>
    // /// Advance to Zoo Pig State so the Metal Worm can be returned anytime
    // /// </summary>
    // [HarmonyPatch(typeof(CopySaveTrigger), "Load")]
    // public static class AutoAdvanceZooPigState
    // {
    //     [HarmonyPostfix]
    //     public static void Postfix(CopySaveTrigger __instance)
    //     {
    //         if(!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame) return;
    //         if (__instance.name != "pig01 (4)") return;
    //         if (!__instance.saveTrigger || __instance.saveTrigger.id != 202) return;
    //         APLocationChecker.FakeLoadCopySaveTrigger(__instance, null, true);
    //     }
    // }
}