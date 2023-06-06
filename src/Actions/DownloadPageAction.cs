// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.Extensions;
using UkrGuru.Extensions.Data;
using UkrGuru.Extensions.Logging;

namespace UkrGuru.WebJobs.Actions;

/// <summary>
/// Represents an action that downloads a page from a specified URL.
/// </summary>
public class DownloadPageAction : BaseAction
{
    /// <summary>
    /// Executes the download page action asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.The value of the TResult parameter contains a boolean value indicating whether the action was successful.</returns>
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var url = More.GetValue("url").ThrowIfBlank("url");

        var filename = GetLocalFileName(More.GetValue("filename") ?? "file.txt");

        var result_name = More.GetValue("result_name") ?? "next_body";

        await DbLogHelper.LogDebugAsync(nameof(DownloadPageAction), new { jobId = JobId, url, filename, result_name }, cancellationToken);

        var content = null as string;

        using HttpClient client = new();

        content = await client.GetStringAsync(url, cancellationToken);

        content = await DbFileHelper.SetAsync(content, filename, false, cancellationToken: cancellationToken);

        await DbLogHelper.LogInformationAsync(nameof(DownloadPageAction), new { jobId = JobId, result = "OK", content }, cancellationToken);

        if (!string.IsNullOrEmpty(result_name)) More[result_name] = content;

        return true;
    }
}
