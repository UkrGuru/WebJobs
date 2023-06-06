// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace UkrGuru.WebJobs.Data;

/// <summary>
/// Specifies the possible status values for a job.
/// </summary>
public enum JobStatus
{
    /// <summary>
    /// Indicates that the status of the job is unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Indicates that the job has been queued for processing.
    /// </summary>
    Queued = 1,

    /// <summary>
    /// Indicates that the job is currently running.
    /// </summary>
    Running = 2,

    /// <summary>
    /// Indicates that the job has completed successfully.
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Indicates that the job has failed.
    /// </summary>
    Failed = 4,

    /// <summary>
    /// Indicates that the job has been cancelled.
    /// </summary>
    Cancelled = 5
}