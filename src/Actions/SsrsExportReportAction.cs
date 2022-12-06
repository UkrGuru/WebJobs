// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net;
using System.Text.Json.Serialization;
using System.Web;
using UkrGuru.Extensions;
using UkrGuru.SqlJson;

namespace UkrGuru.WebJobs.Actions;

/// <summary>
/// 
/// </summary>
public class SsrsExportReportAction : BaseAction
{
    /// <summary>
    /// 
    /// </summary>
    public class SsrsSettings
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("baseUrl")]
        public string? BaseUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("userName")]
        public string? UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var ssrs_settings_name = More.GetValue("ssrs_settings_name").ThrowIfBlank("ssrs_settings_name");

        var ssrs_settings = await DbHelper.FromProcAsync<SsrsSettings>("WJbSettings_Get", ssrs_settings_name, cancellationToken: cancellationToken);
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
            url += $"&data={HttpUtility.UrlEncode(data)}";
        }

        await WJbLogHelper.LogDebugAsync(nameof(DownloadPageAction), new { jobId = JobId, url, timeout, filename, result_name }, cancellationToken);

        // WebClient is obsolete
        // using WebClient client = new();
        // client.Credentials = new NetworkCredential(ssrs_settings.UserName, ssrs_settings.Password);
        // file.FileContent = await client.DownloadDataTaskAsync(url);

        var handler = new HttpClientHandler();
        if (!string.IsNullOrEmpty(ssrs_settings.UserName) && !string.IsNullOrEmpty(ssrs_settings.Password))
        {
            handler.PreAuthenticate = true;
            handler.Credentials = new NetworkCredential(ssrs_settings.UserName, ssrs_settings.Password);
        }

        var client = new HttpClient(handler) { BaseAddress = new Uri(ssrs_settings.BaseUrl), Timeout = TimeSpan.FromSeconds(timeout) };

        var response = await client.GetAsync(new Uri(url), cancellationToken);

        response.EnsureSuccessStatusCode();

         WJbFile file = new()
        {
            FileName = filename,
            FileContent = await response.Content.ReadAsByteArrayAsync(cancellationToken)
        };

        var guid = await file.SetAsync(cancellationToken);

        if (!string.IsNullOrEmpty(result_name))
        {
            More[result_name] = guid;
        }

        await WJbLogHelper.LogInformationAsync(nameof(DownloadPageAction), new { jobId = JobId, result = "OK", guid }, cancellationToken);

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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
