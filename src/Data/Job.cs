// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace UkrGuru.WebJobs.Data;

/// <summary>
/// 
/// </summary>
public partial class JobQueue : Job { }

/// <summary>
/// 
/// </summary>
public partial class JobHistory : Job { }

/// <summary>
/// 
/// </summary>
public partial class Job : Rule
{
    /// <summary>
    /// 
    /// </summary>
    [Display(Name = "Id")]
    public int JobId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Display(Name = "Priority")]
    public Priorities JobPriority { get; set; } = Priorities.Normal;

    /// <summary>
    /// 
    /// </summary>
    [DisplayFormat(DataFormatString = "{0:HH:mm:ss.fff}")]
    public DateTime Created { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayFormat(DataFormatString = "{0:HH:mm:ss.fff}")]
    public DateTime? Started { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayFormat(DataFormatString = "{0:HH:mm:ss.fff}")]
    public DateTime? Finished { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Display(Name = "More")]
    public string? JobMore { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Display(Name = "Status")]
    public JobStatus JobStatus { get; set; }
}
