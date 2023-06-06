// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net;
using System.Net.Mail;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using UkrGuru.Extensions;
using UkrGuru.Extensions.Data;
using UkrGuru.Extensions.Logging;
using UkrGuru.SqlJson;

namespace UkrGuru.WebJobs.Actions;

/// <summary>
/// Represents an action that sends an email.
/// </summary>
public class SendEmailAction : BaseAction
{
    /// <summary>
    /// Represents the settings for an SMTP server.
    /// </summary>
    public class SmtpSettings
    {
        /// <summary>
        /// Gets or sets the email address of the sender.
        /// </summary>
        [JsonPropertyName("from")]
        public string? From { get; set; }

        /// <summary>
        /// Gets or sets the host name or IP address of the SMTP server.
        /// </summary>
        [JsonPropertyName("host")]
        public string? Host { get; set; }

        /// <summary>
        /// Gets or sets the port number used by the SMTP server.
        /// </summary>
        [JsonPropertyName("port")]
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SSL is used to encrypt the connection.
        /// </summary>
        [JsonPropertyName("enableSsl")]
        public bool EnableSsl { get; set; }

        /// <summary>
        /// Gets or sets the user name used to authenticate with the SMTP server.
        /// </summary>
        [JsonPropertyName("userName")]
        public string? UserName { get; set; }

        /// <summary>
        /// Gets or sets the password used to authenticate with the SMTP server.
        /// </summary>
        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }

    /// <summary>
    /// Executes the send email action asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.The value of the TResult parameter contains a boolean value indicating whether the action was successful.</returns>
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var smtp_settings_name = More.GetValue("smtp_settings_name").ThrowIfBlank("smtp_settings_name");

        var smtp_settings = await DbHelper.ExecAsync<SmtpSettings>("WJbSettings_Get", smtp_settings_name, cancellationToken: cancellationToken);
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

        await DbLogHelper.LogDebugAsync(nameof(SendEmailAction), new
        {
            jobId = JobId,
            to,
            cc,
            bcc,
            subject,
            body = ShortStr(body, 200),
            attachments
        }, cancellationToken);

        if (Guid.TryParse(body, out var guidBody))
            body = await DbFileHelper.GetAsync(body, cancellationToken: cancellationToken);

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
                    var file = await DbFileHelper.GetAsync(guidAttach, cancellationToken: cancellationToken);
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

        await DbLogHelper.LogInformationAsync(nameof(SendEmailAction), new { jobId = JobId, result = "OK" }, cancellationToken);

        return true;
    }

    /// <summary>
    /// Determines whether the specified string is an HTML body.
    /// </summary>
    /// <param name="body">The string to check.</param>
    /// <returns>A boolean value indicating whether the specified string is an HTML body.</returns>
    public static bool IsHtmlBody(string? body) => body != null && Regex.IsMatch(body, @"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");  // or @"<[^>]+>"

//    public static bool IsHtmlBody(string? body)
//    {
//        return !string.IsNullOrEmpty(body) && Regex.IsMatch(body, @"<\s*([a-z]+)(?:\s+[^>]*)?>.*?<\s*/\s*\1\s*>", RegexOptions.IgnoreCase);
//    }
//}
