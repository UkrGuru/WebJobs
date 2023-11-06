// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.SqlJson;
using UkrGuru.SqlJson.Extensions;

namespace UkrGuru.WebJobs.Actions;

/// <summary>
/// Represents an action that runs a SQL procedure.
/// </summary>
public class RunSqlProcAction : BaseAction
{
    /// <summary>
    /// Executes the run SQL procedure action asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.The value of the TResult parameter contains a boolean value indicating whether the action was successful.</returns>
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var proc = More.GetValue("proc").ThrowIfBlank("proc");

        var data = More.GetValue("data");

        var timeout = More.GetValue("timeout", (int?)null);

        var result_name = More.GetValue("result_name");

        await DbLogHelper.LogDebugAsync(nameof(RunSqlProcAction), new { jobId = JobId, proc, data = ShortStr(data, 200), result_name, timeout }, cancellationToken);

        if (result_name?.Length > 0)
        {
            var result = await DbHelper.ExecAsync<string?>($"WJb_{proc}", data, timeout, cancellationToken);

            await DbLogHelper.LogInformationAsync(nameof(RunSqlProcAction), new { jobId = JobId, result = ShortStr(result, 200) }, cancellationToken);

            More[result_name] = result;

            return GetDecision(result);
        }
        else
        {
            await DbHelper.ExecAsync($"WJb_{proc}", data, timeout, cancellationToken);

            await DbLogHelper.LogInformationAsync(nameof(RunSqlProcAction), new { jobId = JobId, result = "OK" }, cancellationToken);
        }

        return true;
    }
}
