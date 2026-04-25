using System;
using System.Linq;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using CornKidzAP.Archipelago;
using HarmonyLib;
using Random = UnityEngine.Random;

namespace CornKidzAP.Patches;

public class RandomWordsPatches
{
    [HarmonyPatch(typeof(RandomWords), "Start")]
    public static class RandomWordsAdditions
    {
        // 0 => itemName
        // 1 => locationName
        // 2 => otherPlayerName
        // 3 => itemClass
        // 4 => way of the hero
        private static string[] OffWorldLines =
        [
            "A {0} lies at {1} in {2}'s world.",
            "{2} took your {0}!",
            "Tell {2} to hurry up and get your {0}.",
            "{2} has your {0} at {1}.",
            "{2} should look at {1} for a {3} item for you."
        ];

        private static string[] OnWorldLines =
        [
            "They say you can find {0} at {1} for {2}.",
            "{1} is {4}.",
            "{2} is waiting for their {0} from {1}.",
            "I had a vision of {2}'s {0} at {1}."
        ];

        [HarmonyPostfix]
        public static void Postfix(RandomWords __instance, Words ___wordsCS)
        {
            if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
                return;

            if (!string.Equals(__instance.transform.parent?.name, "priest", StringComparison.Ordinal) || !___wordsCS)
                return;


            var soundClip = ___wordsCS.textLine.First().soundClip;

            TextLine HintToTextLine(Hint hint)
            {
                var itemName = ArchipelagoClient.Session.Items.GetItemName(hint.ItemId);
                var locationName = ArchipelagoClient.Session.Locations.GetLocationNameFromId(hint.LocationId);
                if (!string.IsNullOrEmpty(hint.Entrance))
                {
                    locationName += $" ({hint.Entrance})";
                }

                var itemClass = hint.ItemFlags.HasFlag(ItemFlags.Advancement) ? "progression" : hint.ItemFlags.HasFlag(ItemFlags.NeverExclude) ? "useful" : hint.ItemFlags.HasFlag(ItemFlags.Trap) ? "trap" : "filler";
                var wayOfTheHero = hint.ItemFlags.HasFlag(ItemFlags.Advancement) ? "on the way of the hero" : "useless";

                string otherPlayerName;
                string text;
                if (hint.FindingPlayer != ArchipelagoClient.Session.ConnectionInfo.Slot)
                {
                    otherPlayerName = ArchipelagoClient.Session.Players.GetPlayerAlias(hint.FindingPlayer);
                    text = OffWorldLines[Random.RandomRangeInt(0, OffWorldLines.Length)];
                }
                else
                {
                    otherPlayerName = ArchipelagoClient.Session.Players.GetPlayerAlias(hint.ReceivingPlayer);
                    text = OnWorldLines[Random.RandomRangeInt(0, OnWorldLines.Length)];
                }

                text = string.Format(text, itemName, locationName, otherPlayerName, itemClass, wayOfTheHero);
                return new TextLine
                {
                    textys = text.ToUpper(),
                    soundClip = soundClip,
                    choice = []
                };
            }

            var lines = ArchipelagoClient.Hints.Keys.Where(x => x.Status != HintStatus.Found).Select(HintToTextLine).ToArray();
            if (!lines.Any())
                return;

            ___wordsCS.textLine = lines;
        }
    }
}