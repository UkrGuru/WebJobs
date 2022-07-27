// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions;

public class ParseTextAction : BaseAction
{
    public class ParsingGoal
    {
        [Key]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("parent")]
        public string? Parent { get; set; }

        [JsonPropertyName("start")]
        public string? Start { get; set; }

        [JsonPropertyName("end")]
        public string? End { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }

        public ParsingGoal(string name, string? parent = default, string? start = default, string? end = default) {
            Name = name; Parent = parent;  Start = start; End = end; 
        }
    }

    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var text = More.GetValue("text");
        ArgumentNullException.ThrowIfNull(text);

        var goals = (ParsingGoal[]?)More.GetValue("goals", (object[]?)null);
        ArgumentNullException.ThrowIfNull(goals);

        ParsingGoalExtensions.AppendRootNode(ref goals, text);

        for(int i = 0; i < goals.Length; i++)
        {
            goals[i].Value = goals.ParseValue(goals[i]);
        }

        await LogHelper.LogInformationAsync(nameof(SendEmailAction), new { jobId = JobId, result = "OK" }, cancellationToken);

        return true;
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
