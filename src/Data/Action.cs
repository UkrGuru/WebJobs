// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace UkrGuru.WebJobs.Data;

/// <summary>
/// Represents an action that can be performed by a job.
/// </summary>
public partial class Action
{
    /// <summary>
    /// Gets or sets the ID of the action.
    /// </summary>
    [Display(Name = "Id")]
    public int ActionId { get; set; }

    /// <summary>
    /// Gets or sets the name of the action.
    /// </summary>
    [Required]
    [StringLength(100)]
    [Display(Name = "Action")]
    public string? ActionName { get; set; }

    /// <summary>
    /// Gets or sets the type of the action. For example: "RunSqlProcAction, UkrGuru.WebJobs" or "YourNamespace.RunYourSqlProcAction, YourApp".
    /// If the namespace is not specified, the default namespace "UkrGuru.WebJobs.Actions" will be used.
    /// </summary>
    [Required]
    [StringLength(255)]
    [Display(Name = "Type")]
    public string? ActionType { get; set; }

    /// <summary>
    /// Gets or sets additional information about the action.
    /// </summary>
    [Display(Name = "More")]
    public string? ActionMore { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the action is disabled.
    /// </summary>
    public bool Disabled { get; set; }
}