// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace UkrGuru.WebJobs.Data;

/// <summary>
/// Provides extension methods for the <see cref="Job"/> class.
/// </summary>
public static class JobExtensions
{
    /// <summary>
    /// Creates an action object for the specified <see cref="Job"/>.
    /// </summary>
    /// <param name="job">The <see cref="Job"/> for which to create an action object.</param>
    /// <returns>An action object for the specified <see cref="Job"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <see cref="Action.ActionType"/> property of the <paramref name="job"/> is <c>null</c>.
    /// </exception>
    public static dynamic? CreateAction(this Job job)
    {
        ArgumentNullException.ThrowIfNull(job.ActionType);

        var type = Type.GetType($"UkrGuru.WebJobs.Actions.{job.ActionType}") ?? Type.GetType(job.ActionType);
        ArgumentNullException.ThrowIfNull(type);

        dynamic? action = Activator.CreateInstance(type);

        action?.Init(job);

        return action;
    }
}
