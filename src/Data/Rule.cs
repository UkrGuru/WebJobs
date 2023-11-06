// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace UkrGuru.WebJobs.Data;

/// <summary>
/// Represents a rule that can be associated with an action.
/// </summary>
public partial class Rule: Action
{
    /// <summary>
    /// Gets or sets the ID of the rule.
    /// </summary>
    [Display(Name = "Id")]
    public int RuleId { get; set; }

    /// <summary>
    /// Gets or sets the name of the rule.
    /// </summary>
    [Display(Name = "Rule")]
    public string? RuleName { get; set; }

    /// <summary>
    /// Gets or sets the priority of the rule.
    /// </summary>
    [Display(Name = "Priority")]
    public Priority RulePriority { get; set; } = Priority.Normal;

    /// <summary>
    /// Gets or sets additional information about the rule.
    /// </summary>
    [Display(Name = "More")]
    public string? RuleMore { get; set; }
}
