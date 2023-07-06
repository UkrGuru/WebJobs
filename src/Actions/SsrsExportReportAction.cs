// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net;
using System.Text.Json.Serialization;
using System.Web;
using UkrGuru.Extensions;
using UkrGuru.Extensions.Data;
using UkrGuru.Extensions.Logging;
using UkrGuru.SqlJson;

namespace UkrGuru.WebJobs.Actions;

/// <summary>
/// Represents an action that exports an SSRS report.
/// </summary>
public class SsrsExportReportAction : BaseAction
{
    /// <summary>
    /// Represents the settings for an SSRS server.
    /// </summary>
    public class SsrsSettings
    {
        /// <summary>
        /// Gets or sets the base URL of the SSRS server.
        /// </summary>
        [JsonPropertyName("baseUrl")]
        public string? BaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the user name used to authenticate with the SSRS server.
        /// </summary>
        [JsonPropertyName("userName")]
        public string? UserName { get; set; }

        /// <summary>
        /// Gets or sets the password used to authenticate with the SSRS server.
        /// </summary>
        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }

    /// <summary>
    /// Executes the export SSRS report action asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.The value of the TResult parameter contains a boolean value indicating whether the action was successful.</returns>
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var ssrs_settings_name = More.GetValue("ssrs_settings_name").ThrowIfBlank("ssrs_settings_name");

        var ssrs_settings = await DbHelper.ExecAsync<SsrsSettings>("WJbSettings_Get", ssrs_settings_name, cancellationToken: cancellationToken);
        ArgumentNullException.ThrowIfNull(ssrs_settings?.BaseUrl);

        var report = More.GetValue("report").ThrowIfBlank("report");

        var data = More.GetValue("data") ?? string.Empty;

        int timeout = More.GetValue("timeout", 30);

        var filename = More.GetValue("filename");
        if (!string.IsNullOrEmpty(filename) && filename.Contains("{0:"))
        {
            filename = string.Format(filename, DateTime.Now);
        }

        var format = GetReportFormat(filename);

        var result_name = More.GetValue("result_name");

        var url = $"{ssrs_settings.BaseUrl}{HttpUtility.UrlPathEncode(report)}&rs:Command=Render&rs:Format={format}";

        if (!string.IsNullOrEmpty(data))
        {
            url += $"&Data={HttpUtility.UrlEncode(data)}";
        }

        await DbLogHelper.LogDebugAsync(nameof(DownloadPageAction), new { jobId = JobId, url, timeout, filename, result_name }, cancellationToken);

        var handler = new HttpClientHandler();
        if (!string.IsNullOrEmpty(ssrs_settings.UserName) && !string.IsNullOrEmpty(ssrs_settings.Password))
        {
            handler.PreAuthenticate = true;
            handler.Credentials = new NetworkCredential(ssrs_settings.UserName, ssrs_settings.Password);
        }

        var client = new HttpClient(handler) { BaseAddress = new Uri(ssrs_settings.BaseUrl), Timeout = TimeSpan.FromSeconds(timeout) };

        var response = await client.GetAsync(new Uri(url), cancellationToken);

        response.EnsureSuccessStatusCode();

        DbFile file = new()
        {
            FileName = filename,
            FileContent = await response.Content.ReadAsByteArrayAsync(cancellationToken)
        };

        var guid = await file.SetAsync<Guid?>(cancellationToken: cancellationToken);

        if (!string.IsNullOrEmpty(result_name))
        {
            More[result_name] = guid;
        }

        await DbLogHelper.LogInformationAsync(nameof(DownloadPageAction), new { jobId = JobId, result = "OK", guid }, cancellationToken);

        return true;
    }

    /// <summary>
    /// Gets the report format based on the specified file name.
    /// </summary>
    /// <param name="filename">The file name to use to determine the report format.</param>
    /// <returns>A string representing the report format.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the file extension is not recognized.</exception>
    public static string GetReportFormat(string? filename)
    {
        return (Path.GetExtension(filename)?.ToLower()) switch
        {
            ".docx" => "WORDOPENXML",
            ".xlsx" => "EXCELOPENXML",
            ".pptx" => "PPTX",
            ".pdf" => "PDF",
            ".tif" or ".tiff" => "IMAGE",
            ".mhtml" => "MHTML",
            ".csv" => "CSV",
            ".xml" => "XML",
            ".atom" => "ATOM",
            _ => throw new ArgumentOutOfRangeException(filename)
        };
    }
}
