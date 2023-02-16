// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UkrGuru.WebJobs.Data;

/// <summary>
/// 
/// </summary>
public class ParsingGoal
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("parent")]
    public string? Parent { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("start")]
    public string? Start { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("end")]
    public string? End { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}
