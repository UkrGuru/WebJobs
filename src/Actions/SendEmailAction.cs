// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net;
using System.Net.Mail;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using UkrGuru.Extensions;
using UkrGuru.SqlJson;

namespace UkrGuru.WebJobs.Actions;

/// <summary>
/// 
/// </summary>
public class SendEmailAction : BaseAction
{
    /// <summary>
    /// 
    /// </summary>
    public class SmtpSettings
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("from")]
        public string? From { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("host")]
        public string? Host { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("port")]
        public int Port { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("enableSsl")]
        public bool EnableSsl { get; set; }

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
        var smtp_settings_name = More.GetValue("smtp_settings_name").ThrowIfBlank("smtp_settings_name");

        var smtp_settings = await DbHelper.FromProcAsync<SmtpSettings>("WJbSettings_Get", smtp_settings_name, cancellationToken: cancellationToken);
        ArgumentNullException.ThrowIfNull(smtp_settings);

        var from = More.GetValue("from");
        if (string.IsNullOrEmpty(from)) from = smtp_settings.From;
        ArgumentNullException.ThrowIfNull(from);

        var to = More.GetValue("to");
        ArgumentNullException.ThrowIfNull(to);

        var cc = More.GetValue("cc");
        var bcc = More.GetValue("bcc");

        var subject = More.GetValue("subject");
        var body = More.GetValue("body");

        var attachment = More.GetValue("attachment");
        var attachments = More.GetValue("attachments", (object[]?)null);
        if (attachments == null && !string.IsNullOrEmpty(attachment)) attachments = new[] { attachment };

        await WJbLogHelper.LogDebugAsync(nameof(SendEmailAction), new { jobId = JobId, to, cc, bcc, subject, 
            body = ShortStr(body, 200), attachments }, cancellationToken);

        if (Guid.TryParse(body, out var guidBody))
            body = await WJbFileHelper.GetAsync(body, cancellationToken);

        MailMessage message = new(from, to, subject, body)
        {
            IsBodyHtml = IsHtmlBody(body)
        };

        if (!string.IsNullOrEmpty(cc)) message.CC.Add(cc);

        if (!string.IsNullOrEmpty(bcc)) message.Bcc.Add(bcc);

        if (attachments != null && attachments.Length > 0)
        {
            foreach (var fileName in attachments)
            {
                attachment = Convert.ToString(fileName);
                ArgumentNullException.ThrowIfNull(attachment);

                if (Guid.TryParse(attachment, out var guidAttach))
                {
                    var file = await WJbFileHelper.GetAsync(guidAttach, cancellationToken);
                    ArgumentNullException.ThrowIfNull(file);
                    ArgumentNullException.ThrowIfNull(file?.FileContent);

                    message.Attachments.Add(new Attachment(new MemoryStream(file.FileContent), file.FileName));
                }
                else 
                {
                    message.Attachments.Add(new Attachment(attachment));
                }
            }
        }

        using var smtp = new SmtpClient(smtp_settings.Host, smtp_settings.Port);

        smtp.EnableSsl = smtp_settings.EnableSsl;
        smtp.Credentials = new NetworkCredential(smtp_settings.UserName, smtp_settings.Password);

        await smtp.SendMailAsync(message, cancellationToken);

        await WJbLogHelper.LogInformationAsync(nameof(SendEmailAction), new { jobId = JobId, result = "OK" }, cancellationToken);

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public static bool IsHtmlBody(string? body) => body != null && Regex.IsMatch(body, @"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");  // or @"<[^>]+>"
}
