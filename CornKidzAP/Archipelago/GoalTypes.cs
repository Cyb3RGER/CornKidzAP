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

[Flags]
public enum GoalSelection
{
    None = 0,
    Owlloh = 1,
    Tower = 2,
    Anxiety = 4,
    God = 8
}

public static class GoalSelectionExtensions
{
    public static GoalSelection ToGoalSelection(this string[] goalNames)
    {
        var selection = GoalSelection.None;
        foreach (var goalName in goalNames)
        {
            var current = goalName switch
            {
                "Defeat Owlloh" => GoalSelection.Owlloh,
                "Climb the Tower" => GoalSelection.Tower,
                "Climb the Anxiety Tower" => GoalSelection.Anxiety,
                "Meet the Dog God" => GoalSelection.God,
                _ => GoalSelection.None
            };
            if (current == GoalSelection.None)
            {
                CornKidzAP.Logger.LogWarning($"Ignored unknown goal {goalName}.");
            }
            selection |= current;
        }

        return selection;
    }

    public static int Count(this GoalSelection selection)
    {
        var count = 0;
        foreach (GoalSelection value in Enum.GetValues(typeof(GoalSelection)))
        {
            if (value != GoalSelection.None && selection.HasFlag(value))
            {
                count++;
            }
        }
        return count;
    }

    public static string Describe(this GoalSelection selection, GoalSelection skip)
    {
        if (selection == GoalSelection.None)
            return string.Empty;

        List<string> texts = [];
        for (var i = 0; i < Enum.GetValues(typeof(GoalSelection)).Length - 1; i++)
        {
            var value = (GoalSelection)(1 << i);
            if (!skip.HasFlag(value) && selection.HasFlag(value))
            {
                texts.Add(value switch
                {
                    GoalSelection.Owlloh => "OWLLOH",
                    GoalSelection.Tower => "TOWER",
                    GoalSelection.Anxiety => "ANXIETY",
                    GoalSelection.God => "DOG GOD",
                    _ => "???"
                });
            }
        }

        return string.Join(",", texts);
    }
}

public static class GoalTypesExtensions
{
    public static GoalSelection ToGoalSelection(this GoalTypes goal)
    {
        return goal switch
        {
            GoalTypes.Owlloh => GoalSelection.Owlloh,
            GoalTypes.Tower => GoalSelection.Tower,
            GoalTypes.Anxiety => GoalSelection.Anxiety,
            GoalTypes.God => GoalSelection.God,
            _ => throw new ArgumentOutOfRangeException(nameof(goal), goal, null)
        };
    }
}