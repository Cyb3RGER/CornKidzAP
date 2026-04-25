using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace CornKidzAP.Archipelago;

public class APSlotData(Dictionary<string, object> slotData)
{
    public object this[string key] => slotData[key];

    public int GetInt(string key, int _default = 0)
    {
        if (!slotData.TryGetValue(key, out var value))
            return _default;
        return int.TryParse(value.ToString(), out var result) ? result : _default;
    }

    public T[] GetSet<T>(string key)
    {
        if (!slotData.TryGetValue(key, out var value))
            return [];
        return ((JArray)value).ToObject<T[]>();
    }
    
    [CanBeNull]
    public Version GetVersion(string key, [CanBeNull] Version _default = null)
    {
        if (!slotData.TryGetValue(key, out var value))
            return _default;
        var didParse = Version.TryParse(value.ToString(), out var result);
        return didParse ? result : _default;
    }

    public GoalSelection Goals
    {
        get
        {
            var values = GetSet<string>("goal_selection");
            if (values.Length == 0 && slotData.ContainsKey("goal"))
            {
#pragma warning disable CS0612 // Type or member is obsolete - Downward compatibility
                return Goal.ToGoalSelection();
#pragma warning restore CS0612 // Type or member is obsolete
            }
            return values.ToGoalSelection();
        }
    }

    [Obsolete]
    public GoalTypes Goal
    {
        get
        {
            var value = GetInt("goal", 1);
            return Enum.IsDefined(typeof(GoalTypes), value) ? (GoalTypes)value : GoalTypes.Tower;
        }
    }

    [CanBeNull] public Version ServerVersion => GetVersion("version");
    public int MaxHP => GetInt("max_hp", 8);
    public bool IsCranksanity => GetInt("cranksanity") > 0;
    public bool IsRatsanity => GetInt("ratsanity") > 0;
    public bool IsAchievementsanity => GetInt("achievementsanity") > 0;
    public bool IsFishsanity => GetInt("fishsanity") > 0;
    
    public bool IsDeathLink => GetInt("death_link") > 0;
    public bool IsOpenWollowsHollow => GetInt("open_wollows_hollow") > 0;
    public bool IsMovesanity => GetInt("movesanity") > 0;
    public bool IsSwitchsanity => GetInt("switchsanity") > 0;
    public bool IsTestCubesanity => GetInt("test_cubesanity") > 0;
}