﻿// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;
using UkrGuru.SqlJson.Extensions;
using UkrGuru.WebJobs.Data;
using static UkrGuru.WebJobs.Data.ParsingGoalExtensions;

namespace UkrGuru.WebJobs.Actions;

/// <summary>
/// Represents an action that parses text according to specified goals.
/// </summary>
public class ParseTextAction : BaseAction
{
    /// <summary>
    /// Executes the parse text action asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.The value of the TResult parameter contains a boolean value indicating whether the action was successful.</returns>
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var text = More.GetValue("text");
        ArgumentNullException.ThrowIfNull(text);

        var goals = JsonSerializer.Deserialize<ParsingGoal[]>(More.GetValue("goals") ?? "[]");
        ArgumentNullException.ThrowIfNull(goals);

        var result_name = More.GetValue("result_name");

        goals = goals.AppendRootNode(text);

        for (int i = 0; i < goals.Length; i++)
        {
            goals[i].Value = goals.ParseValue(goals[i]);
        }

        var result = goals.GetResult();

        if (result_name?.Length > 0)
        {

            More[result_name] = result;

            await DbLogHelper.LogInformationAsync(nameof(SendEmailAction), new { jobId = JobId, result = ShortStr(result, 200) }, cancellationToken);

            return GetDecision(result);
        }
        else
        {
            await DbLogHelper.LogInformationAsync(nameof(SendEmailAction), new { jobId = JobId, result = "OK" }, cancellationToken);
        }

        return true;
    }
}