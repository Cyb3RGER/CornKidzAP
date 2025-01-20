using HarmonyLib;
using UnityEngine;

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

    /// <summary>
    /// Locally (un)flip the ooze collider in the zoo to avoid y-shift
    /// </summary>
    [HarmonyPatch(typeof(SaveTrigger), nameof(SaveTrigger.Load))]
    public static class FixZooOoze
    {
        [HarmonyPostfix]
        public static void Postfix(SaveTrigger __instance)
        {
            if(__instance.name != "flipevent" || __instance.id != 200) return;
            
            var obj = GameObject.Find("WORLD/etcObjects/ooze/ooze (1)");
            if(!obj) return;
            obj.transform.localScale = __instance.bOn ? new(-1,1,1) : Vector3.one;
        }
    }
}