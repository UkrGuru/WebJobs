// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.SqlJson;
using UkrGuru.SqlJson.Extensions;

namespace UkrGuru.WebJobs.Actions;

/// <summary>
/// Represents an action that processes items from a specified file.
/// </summary>
public class ProcItemsAction : BaseAction
{
    /// <summary>
    /// Executes the process items action asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.The value of the TResult parameter contains a boolean value indicating whether the action was successful.</returns>
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var fileId = More.GetValue("fileId").ThrowIfBlank("fileId");

        var itemNo = More.GetValue("itemNo", (int?)null);

        var proc = More.GetValue("proc").ThrowIfBlank("proc");

        var timeout = More.GetValue("timeout", (int?)null);

        int i = itemNo ?? 0;
        while (1 == 1)
        {
            int? result = 0;

            var more = await DbHelper.ReadAsync<string?>("WJbItems_Get_More",
                new { FileId = fileId, ItemNo = i }, cancellationToken: cancellationToken);

            if (more == null) break;

            try
            {
                result = await DbHelper.ExecAsync<int?>($"WJb_{proc}",
                    more, timeout, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                await DbLogHelper.LogErrorAsync(nameof(ProcItemsAction),
                    new { jobId = JobId, fileId, itemNo = i, proc, errMsg = ex.Message }, cancellationToken);
            }
            finally
            {
                await DbHelper.UpdateAsync("WJbItems_Set_Result",
                    new { FileId = fileId, ItemNo = i, Result = result }, timeout, cancellationToken: cancellationToken);
            }

            if (itemNo != null) break; 
            
            i++;
        }

        await DbLogHelper.LogInformationAsync(nameof(ProcItemsAction),
            new { jobId = JobId, result = "OK", count = itemNo > 0 ? 1 : i + 1 }, cancellationToken);

        return true;
    }
}