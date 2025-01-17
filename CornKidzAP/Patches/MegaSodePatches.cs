using CornKidzAP.Archipelago;
using HarmonyLib;

namespace CornKidzAP.Patches;

public class MegaSodePatches
{
    /// <summary>
    /// Patch to fix mega soda
    /// </summary>
    [HarmonyPatch(typeof(SaveItem), nameof(SaveItem.HPup))]
    public static class MegaSodeFix
    {
        [HarmonyPrefix]
        public static bool Prefix(SaveItem __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return true;

            var locId = APLookup.GetAPLocationForSaveItem(__instance);
            return locId == null;
            //just remove vanilla behavior
        }
    }

}