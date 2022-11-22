// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.Extensions;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions;

public class DownloadPageAction : BaseAction
{
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var url = More.GetValue("url").ThrowIfBlank("url");

        var filename = GetLocalFileName(More.GetValue("filename") ?? "file.txt");

        var result_name = More.GetValue("result_name") ?? "next_body";

        await WJbLogHelper.LogDebugAsync(nameof(DownloadPageAction), new { jobId = JobId, url, filename, result_name }, cancellationToken);

        var content = null as string;

        using HttpClient client = new();

        content = await client.GetStringAsync(url, cancellationToken);

        content = await WJbFileHelper.SetAsync(content, filename, false, cancellationToken);

        await WJbLogHelper.LogInformationAsync(nameof(DownloadPageAction), new { jobId = JobId, result = "OK", content }, cancellationToken);

        if (!string.IsNullOrEmpty(result_name)) More[result_name] = content;

        return true;
    }
}
