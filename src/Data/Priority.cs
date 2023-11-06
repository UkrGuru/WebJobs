// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace UkrGuru.WebJobs.Data;

/// <summary>
/// Specifies the priority levels for a rule. 
/// </summary>
public enum Priority
{
    /// <summary>
    /// Indicates that the rule should be processed as soon as possible.
    /// </summary>
    ASAP,
    /// <summary>
    /// Indicates that the rule has a high priority.
    /// </summary>
    High,
    /// <summary>
    /// Indicates that the rule has a normal priority.
    /// </summary>
    Normal,
    /// <summary>
    /// Indicates that the rule has a low priority.
    /// </summary>
    Low
}