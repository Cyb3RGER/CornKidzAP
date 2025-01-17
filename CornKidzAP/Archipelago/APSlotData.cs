using System;
using System.Collections.Generic;

namespace CornKidzAP.Archipelago;

public enum GoalTypes
{
    Owlloh,
    Tower,
    Anxiety,
    God
}

public class APSlotData(Dictionary<string, object> slotData)
{
    public object this[string key] => slotData[key];

    public int GetInt(string key, int _default = 0)
    {
        if (!slotData.TryGetValue(key, out var value))
            return _default;
        return int.TryParse(value.ToString(), out var result) ? result : _default;
    }

    public GoalTypes Goal
    {
        get
        {
            var value = GetInt("goal", 1);
            return Enum.IsDefined(typeof(GoalTypes), value) ? (GoalTypes)value : GoalTypes.Tower;
        }
    }

    public int MaxHP => GetInt("max_hp", 8);
    public bool IsCranksanity => GetInt("cranksanity") > 0;
    public bool IsRatsanity => GetInt("ratsanity") > 0;
    public bool IsAchievementsanity => GetInt("achievementsanity") > 0;
    public bool IsFishsanity => GetInt("fishsanity") > 0;
    
    public bool IsDeathLink => GetInt("death_link") > 0;
    public bool IsOpenMode => GetInt("open_mode") > 0;
}