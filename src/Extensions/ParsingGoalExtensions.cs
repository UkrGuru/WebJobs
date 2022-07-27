// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;

namespace UkrGuru.WebJobs.Data;

public static class ParsingGoalExtensions
{
    public static ParsingGoal[] AppendRootNode(this ParsingGoal[] goals, string text)
    {
        var root = goals.FirstOrDefault(e => e.Name == string.Empty);

        if (root != null) throw new Exception("It is not possible to add text as a root node because it is already present.");

        for (int i = 0; i < goals.Length; i++) goals[i].Parent ??= string.Empty;

        return goals.Append(new ParsingGoal() { Name= "",  Value = text }).ToArray();
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

    public static bool IsLeaf(this ParsingGoal[] goals, string name) => !goals.Any(e => e.Parent == name);

    public static string? GetResult(this ParsingGoal[] goals)
    {
        var dict = new Dictionary<string, object?>();
        
        foreach (var goal in goals.Where(goal => goals.IsLeaf(goal.Name)))
        {
            dict.Add(goal.Name, goal.Value);
        }

        return JsonSerializer.Serialize(dict);
    }

    public static string? Crop(string? text, string? start, string? end = default)
    {
        if (text == null) return null;

        string? result;

        var startIndex = string.IsNullOrEmpty(start) ? 0 : text.IndexOf(start);

        if (startIndex < 0) return null;

        startIndex += start?.Length ?? 0;

        if (string.IsNullOrEmpty(end))
        {
            result = text.Substring(startIndex);
        }
        else
        {
            var endIndex = text.IndexOf(end, startIndex);

            if (endIndex < 0) return null;

            result = text.Substring(startIndex, endIndex - startIndex);
        }

        result = result.Trim(new char[] { ' ', '\t', '\r', '\n' });

        return result;
    }
}