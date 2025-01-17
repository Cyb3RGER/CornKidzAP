using CornKidzAP.Archipelago;
using HarmonyLib;

namespace CornKidzAP.Patches;

public class ResultsPatches
{
    [HarmonyPatch(typeof(Results), "Start")]
    public static class GoalCheckerResults
    {
        [HarmonyPostfix]
        public static void Postfix(Results __instance)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;
            var parkItemCount = 0;
            for (var index = 100; index < 200; index++)
            {
                if (index is 145 or 146 or 147) continue;
                if (!CheckItemCollected(index)) continue;
                parkItemCount++;
            }

            var hollowItemCount = 0;
            for (var index = 200; index < 321; index++)
            {
                if (!CheckItemCollected(index)) continue;

                hollowItemCount++;
            }

            __instance.parkXPtext.text = $"{parkItemCount}/53";
            __instance.hollowXPtext.text = $"{hollowItemCount}/103";
        }

        private static bool CheckItemCollected(int index)
        {
            var locID = APLookup.GetAPLocationForSaveItemId(index);
            return locID == null ?
                //not an AP location, vanilla check for item
                GameCtrl.instance.data.items[index] :
                // AP location check
                ArchipelagoClient.Session.Locations.AllLocationsChecked.Contains(locID.Value);
        }
    }
}