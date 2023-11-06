// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;
using System.Text.Json.Serialization;
using UkrGuru.SqlJson;
using UkrGuru.SqlJson.Extensions;

namespace UkrGuru.WebJobs.Actions;

/// <summary>
/// Represents an action that runs an API procedure.
/// </summary>
public class SendHttpRequestAction : BaseAction
{
    /// <summary>
    /// Represents the settings for an API.
    /// </summary>
    public class HttpSettings
    {
        /// <summary>
        /// Gets or sets the URL of the API.
        /// </summary>
        [JsonPropertyName("url")]
        public string? BaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the headers for accessing the API.
        /// </summary>
        [JsonPropertyName("headers")]
        public More? BaseHeaders { get; set; }
    }

    /// <summary>
    /// Executes the run any API request action asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    /// <exception cref="Exception">A task that represents the asynchronous operation.The value of the TResult parameter contains a boolean value indicating whether the action was successful.</exception>
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var http_settings_name = More.GetValue("http_settings_name").ThrowIfBlank("http_settings_name");

        var http_settings = await WJbDbHelper.WJbSettings_GetAsync<HttpSettings>(http_settings_name, cancellationToken);
        ArgumentNullException.ThrowIfNull(http_settings);

        var method = More.GetValue("method") ?? "GET";
        var requestUrl = More.GetValue("url");

        var headers = More.GetValue<More>("headers");

        var content = More.GetValue("content");
        var content_type = More.GetValue("content_type");

        var timeout = More.GetValue("timeout", (int?)null);

        var result_name = More.GetValue("result_name");

        await DbLogHelper.LogDebugAsync(nameof(SendHttpRequestAction),
            new { jobId = JobId, method, url = $"{http_settings.BaseUrl}{requestUrl}", timeout, result_name }, cancellationToken);

        var request = new HttpRequestMessage(new HttpMethod(method), requestUrl);

        if (http_settings.BaseHeaders?.Count > 0)
        {
            foreach (var header in http_settings.BaseHeaders) request.Headers.Add(header.Key, Convert.ToString(header.Value));
        }
        if (headers?.Count > 0)
        {
            foreach (var header in headers) request.Headers.Add(header.Key, Convert.ToString(header.Value));
        }

        if (content?.Length > 0)
        {
            request.Content = string.IsNullOrEmpty(content_type) ? new StringContent(content) :
                new StringContent(content, Encoding.UTF8, content_type);

            await DbLogHelper.LogDebugAsync(nameof(SendHttpRequestAction),
                new { jobId = JobId, content = ShortStr(content, 200), content_type }, cancellationToken);
        }

        using var client = new HttpClient();

        if (http_settings?.BaseUrl?.Length > 0) client.BaseAddress = new Uri(http_settings.BaseUrl);

        if (timeout > 0) client.Timeout = new TimeSpan(0, 0, (int)timeout);

        using var response = await client.SendAsync(request, cancellationToken);

        var body = await response.ReadAsync<string?>(cancellationToken);

        await DbLogHelper.LogInformationAsync(nameof(SendHttpRequestAction),
            new { jobId = JobId, result = ShortStr(body, 200) }, cancellationToken);

        if (result_name?.Length > 0)
        {
            More[result_name] = body;

            return GetDecision(body);
        }

        return true;
    }
}
