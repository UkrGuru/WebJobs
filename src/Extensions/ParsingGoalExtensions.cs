// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace UkrGuru.WebJobs.Data;

/// <summary>
/// Provides extension methods for working with ParsingGoal objects. 
/// </summary>
public static class ParsingGoalExtensions
{
    /// <summary>
    /// Represents a parsing goal with a name, parent, start and end markers, and a value.
    /// </summary>
    public class ParsingGoal
    {
        /// <summary>
        /// The name of the parsing goal.
        /// </summary>
        [Key]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The parent of the parsing goal.
        /// </summary>
        [JsonPropertyName("parent")]
        public string? Parent { get; set; } = string.Empty;

        /// <summary>
        /// The start marker of the parsing goal.
        /// </summary>
        [JsonPropertyName("start")]
        public string? Start { get; set; }

        /// <summary>
        /// The end marker of the parsing goal.
        /// </summary>
        [JsonPropertyName("end")]
        public string? End { get; set; }

        /// <summary>
        /// The value of the parsing goal.
        /// </summary>
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    /// <summary>
    /// Appends a root node to the specified array of ParsingGoal objects using the specified text.
    /// </summary>
    /// <param name="goals">The array of ParsingGoal objects to append to.</param>
    /// <param name="text">The text to use for the root node.</param>
    /// <returns>An array of ParsingGoal objects with the appended root node.</returns>
    /// <exception cref="Exception">Thrown when a root node is already present in the array of ParsingGoal objects.</exception>
    public static ParsingGoal[] AppendRootNode(this ParsingGoal[] goals, string text)
    {
        var root = goals.FirstOrDefault(e => e.Name == string.Empty);

        if (root != null) throw new Exception("It is not possible to add text as a root node because it is already present.");

        for (int i = 0; i < goals.Length; i++) goals[i].Parent ??= string.Empty;

        return goals.Append(new ParsingGoal() { Name= "",  Value = text }).ToArray();
    }

    /// <summary>
    /// Parses the value of the specified ParsingGoal object using the specified array of ParsingGoal objects.
    /// </summary>
    /// <param name="goals">The array of ParsingGoal objects to use for parsing.</param>
    /// <param name="goal">The ParsingGoal object to parse.</param>
    /// <returns>The parsed value of the specified ParsingGoal object.</returns>
    /// <exception cref="Exception">Thrown when an unknown parent is encountered.</exception>
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

    /// <summary>
    /// Determines whether the specified name is a leaf node in the specified array of ParsingGoal objects.
    ///</summary> 
    /// <param name="goals">The array of ParsingGoal objects to search.</param>
    /// <param name="name">The name to search for.</param>
    /// <returns>true if the specified name is a leaf node; otherwise, false.</returns>
    public static bool IsLeaf(this ParsingGoal[] goals, string name) => !goals.Any(e => e.Parent == name);

    /// <summary>
    /// Gets the result of parsing the specified array of ParsingGoal objects.
    /// </summary>
    /// <param name="goals">The array of ParsingGoal objects to parse.</param>
    /// <returns>A JSON string representing the result of parsing the specified array of ParsingGoal objects.</returns>
    public static string? GetResult(this ParsingGoal[] goals)
    {
        var dict = new Dictionary<string, object?>();
        
        foreach (var goal in goals.Where(goal => goals.IsLeaf(goal.Name)))
        {
            dict.Add(goal.Name, goal.Value);
        }

        return JsonSerializer.Serialize(dict);
    }

    /// <summary>
    /// Crops the specified text using the specified start and end markers.
    /// </summary>
    /// <param name="text">The text to crop.</param>
    /// <param name="start">The start marker to use for cropping.</param>
    /// <param name="end">The end marker to use for cropping.</param>
    /// <returns>The cropped text.</returns>
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

    //public static string? Crop(string? text, string? start, string? end = default)
    //{
    //    if (text == null) return null;

    //    var pattern = Regex.Escape(start) + "(.*?)" + (end != null ? Regex.Escape(end) : "");
    //    var match = Regex.Match(text, pattern);

    //    return match.Success ? match.Groups[1].Value.Trim() : null;
    //}
}