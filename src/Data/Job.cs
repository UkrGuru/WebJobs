// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace UkrGuru.WebJobs.Data;

/// <summary>
/// Represents a job that has been added to the job queue.
/// </summary>
public partial class JobQueue : Job { }

/// <summary>
/// Represents a job that has been processed and added to the job history.
/// </summary>
public partial class JobHistory : Job { }

/// <summary>
/// Represents a job that can be processed by the system.
/// </summary>
public partial class Job : Rule
{
    /// <summary>
    /// Gets or sets the ID of the job.
    /// </summary>
    [Display(Name = "Id")]
    public int JobId { get; set; }

    /// <summary>
    /// Gets or sets the priority of the job.
    /// </summary>
    [Display(Name = "Priority")]
    public Priority JobPriority { get; set; } = Priority.Normal;

    /// <summary>
    /// Gets or sets the date and time when the job was created.
    /// </summary>
    [DisplayFormat(DataFormatString = "{0:HH:mm:ss.fff}")]
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the job was started.
    /// </summary>
    [DisplayFormat(DataFormatString = "{0:HH:mm:ss.fff}")]
    public DateTime? Started { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the job was finished.
    /// </summary>
    [DisplayFormat(DataFormatString = "{0:HH:mm:ss.fff}")]
    public DateTime? Finished { get; set; }

    /// <summary>
    /// Gets or sets additional information about the job.
    /// </summary>
    [Display(Name = "More")]
    public string? JobMore { get; set; }

    /// <summary>
    /// Gets or sets the status of the job.
    /// </summary>
    [Display(Name = "Status")]
    public JobStatus JobStatus { get; set; }
}
