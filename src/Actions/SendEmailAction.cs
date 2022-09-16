// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net;
using System.Net.Mail;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using UkrGuru.Extensions;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions;

public class SendEmailAction : BaseAction
{
    public class SmtpSettings
    {
        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("host")]
        public string? Host { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("enableSsl")]
        public bool EnableSsl { get; set; }

        [JsonPropertyName("userName")]
        public string? UserName { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }

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

        await LogHelper.LogDebugAsync(nameof(SendEmailAction), new { jobId = JobId, to, cc, bcc, subject, 
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

        await LogHelper.LogInformationAsync(nameof(SendEmailAction), new { jobId = JobId, result = "OK" }, cancellationToken);

        return true;
    }

    public static bool IsHtmlBody(string? body) => body != null && Regex.IsMatch(body, @"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");  // or @"<[^>]+>"
}
