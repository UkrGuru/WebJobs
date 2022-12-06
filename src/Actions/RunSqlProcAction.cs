// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.Extensions;
using UkrGuru.SqlJson;

namespace UkrGuru.WebJobs.Actions;

/// <summary>
/// 
/// </summary>
public class RunSqlProcAction : BaseAction
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var proc = More.GetValue("proc").ThrowIfBlank("proc");

        var data = More.GetValue("data");

        var timeout = More.GetValue("timeout", (int?)null);

        var result_name = More.GetValue("result_name");

        await WJbLogHelper.LogDebugAsync(nameof(RunSqlProcAction), new { jobId = JobId, proc, data = ShortStr(data, 200), result_name, timeout }, cancellationToken);

        if (string.IsNullOrEmpty(result_name))
        {
            _ = await DbHelper.ExecProcAsync($"WJb_{proc}", data, timeout, cancellationToken);

            await WJbLogHelper.LogInformationAsync(nameof(RunSqlProcAction), new { jobId = JobId, result = "OK" }, cancellationToken);
        }
        else
        {
            var result = await DbHelper.FromProcAsync<string?>($"WJb_{proc}", data, timeout, cancellationToken);

            await WJbLogHelper.LogInformationAsync(nameof(RunSqlProcAction), new { jobId = JobId, result = ShortStr(result, 200) }, cancellationToken);

            More[result_name] = result;
        }

        return true;
    }
}
