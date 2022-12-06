// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace UkrGuru.WebJobs.Data;

/// <summary>
/// 
/// </summary>
public partial class Rule: Action
{
    /// <summary>
    /// 
    /// </summary>
    [Display(Name = "Id")]
    public int RuleId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Display(Name = "Rule")]
    public string? RuleName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Display(Name = "Priority")]
    public Priorities RulePriority { get; set; } = Priorities.Normal;

    /// <summary>
    /// 
    /// </summary>
    [Display(Name = "More")]
    public string? RuleMore { get; set; }
}
