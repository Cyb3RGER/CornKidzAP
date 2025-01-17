/*using System;
using System.Linq;
using HarmonyLib;
using Steamworks.Data;

namespace CornKidzAP.Patches;

public class RandomWordsPatches
{
    [HarmonyPatch(typeof(RandomWords), "Start")]
    public static class RandomWordsAdditions
    {
        private static readonly string[] Lines =
        [
            "Shoutout to Simpleflips!".ToUpper(),
            "You might recognize that as Markus, but to complete stranger, at best, that's Paul.".ToUpper(),
            "Gannon's Tower is on the way of the Hero".ToUpper(),
        ];
        
        [HarmonyPostfix]
        public static void Postfix(RandomWords __instance, Words ___wordsCS)
        {
            if(!string.Equals(__instance.transform.parent?.name, "priest", StringComparison.Ordinal) || !___wordsCS)
                return;
            var soundClip = ___wordsCS.textLine.First().soundClip;
            // foreach (var line in Lines)
            // {
            //     if (___wordsCS.textLine.Any(x => string.Equals(x.textys, line, StringComparison.Ordinal)))
            //         continue;
            //     ___wordsCS.textLine = ___wordsCS.textLine.Append(new TextLine
            //     {
            //         textys = line,
            //         soundClip = soundClip,
            //         choice = []
            //     }).ToArray();
            // }
            ___wordsCS.textLine = Lines.Select(x => new TextLine
            {
                textys = x,
                soundClip = soundClip,
                choice = []
            }).ToArray();
        }
    }
}*/