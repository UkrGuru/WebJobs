// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using static UkrGuru.WebJobs.Actions.ParseTextAction;

namespace UkrGuru.WebJobs.Data;

public static class ParsingGoalExtensions
{
    public static void AppendRootNode(ref ParsingGoal[] goals, string text)
    {
        var root = goals.FirstOrDefault(e => e.Name.Equals(string.Empty));

        if (root != null) throw new Exception("It is not possible to add text as a root node because it is already present.");

        for (int i = 0; i < goals.Length; i++) goals[i].Parent ??= string.Empty;

        goals = goals.Append(new ParsingGoal("") { Value = text }).ToArray();
    }

    public static string? ParseValue(this ParsingGoal[] goals, ParsingGoal? goal)
    {
        if (string.IsNullOrEmpty(goal?.Name)) return goal?.Value;

        var parentIndex = Array.FindIndex(goals, v => v.Name.Equals(goal.Parent));
            
        if (parentIndex < 0) throw new Exception($"Unknown parent for name '{goal.Parent}'.");

        if (string.IsNullOrEmpty(goals[parentIndex].Value)) {
            goals[parentIndex].Value = goals.ParseValue(goals[parentIndex]);
        }

        return Crop(goals[parentIndex].Value, goal?.Start, goal?.End);
    }
}