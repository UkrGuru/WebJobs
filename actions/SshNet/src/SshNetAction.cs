// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Renci.SshNet;
using Renci.SshNet.Common;
using System.Text.Json.Serialization;
using UkrGuru.Extensions;
using UkrGuru.SqlJson;

namespace UkrGuru.WebJobs.Actions.SshNet;

public class SshNetAction : BaseAction
{
    public class SshNetSettings
    {
        [JsonPropertyName("host")]
        public string Host { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; } = 22;

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("keyFiles")]
        public PrivateKeyFile[] KeyFiles { get; set; }

        [JsonPropertyName("fingerPrint")]
        public string FingerPrint { get; set; }
    }

    public static string CombinateRemoteFullName(string path, string fileName) => $"{path}/{fileName}";

    public async Task<SftpClient> CreateSftpClient(string sshnet_settings_name, CancellationToken cancellationToken = default)
    {
        SftpClient sftp = null;

        var sshnet_settings = await DbHelper.ExecAsync<SshNetSettings>("WJbSettings_Get", sshnet_settings_name.ThrowIfBlank("sshnet_settings_name"), cancellationToken: cancellationToken);
        ArgumentNullException.ThrowIfNull(sshnet_settings);

        if (sshnet_settings.KeyFiles?.Length > 0)
        {
            sftp = new SftpClient(sshnet_settings.Host, sshnet_settings.Port, sshnet_settings.UserName, sshnet_settings.KeyFiles);
        }
        else
        {
            sftp = new SftpClient(sshnet_settings.Host, sshnet_settings.Port, sshnet_settings.UserName, sshnet_settings.Password);
        }

        if (!string.IsNullOrEmpty(sshnet_settings.FingerPrint))
        {
            // string hexFingerPrint;
            sftp.HostKeyReceived += delegate (object sender, HostKeyEventArgs e)
            {
                var bytes = sshnet_settings.FingerPrint.Split(':').Select(s => Convert.ToByte(s, 16)).ToArray();
                if (e.FingerPrint.SequenceEqual(bytes))
                    e.CanTrust = true;
                else
                    e.CanTrust = false;

                //var fingerPrint = new StringBuilder();
                //foreach (var b in e.FingerPrint) fingerPrint.AppendFormat("{0:x2}", b);
                //hexFingerPrint = String.Join(':', fingerPrint);
            };
        }

        return await Task.FromResult(sftp);
    }
}