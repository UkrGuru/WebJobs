// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace UkrGuru.WebJobs.Data;

/// <summary>
/// 
/// </summary>
public partial class Action
{
    /// <summary>
    /// 
    /// </summary>
    [Display(Name = "Id")]
    public int ActionId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    [StringLength(100)]
    [Display(Name = "Action")]
    public string? ActionName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    [StringLength(255)]
    [Display(Name = "Type")]
    public string? ActionType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Display(Name = "More")]
    public string? ActionMore { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool Disabled { get; set; }
}