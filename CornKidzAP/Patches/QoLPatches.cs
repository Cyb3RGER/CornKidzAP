using HarmonyLib;
using UnityEngine;

namespace CornKidzAP.Patches;

public class QoLPatches
{
    [HarmonyPatch(typeof(PlayerCtrl), "Run")]
    public static class SmokingKillsFaster
    {
        private static bool bNoAir;
        
        [HarmonyPrefix]
        public static void Postfix(bool ___bNoAir)
        {
            bNoAir = ___bNoAir;
        }
        
        [HarmonyPostfix]
        public static void Postfix(PlayerCtrl __instance)
        {
            if (!__instance.floorObj || !bNoAir || __instance.floorObj?.name != "sodaMachine" || GameCtrl.instance.HP <= 0) return;
            GameCtrl.instance.air = Mathf.MoveTowards(GameCtrl.instance.air, -1f, 0.027f);
        }
    }
}