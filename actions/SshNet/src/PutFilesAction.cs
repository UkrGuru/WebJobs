// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.Extensions;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions.SshNet;

public class PutFilesAction : SshNetAction
{
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        const string funcName = "SshNet.PutFiles";

        var jobId = JobId;

        var sshnet_settings_name = More.GetValue("sshnet_settings_name").ThrowIfBlank("sshnet_settings_name");

        var remote_path = More.GetValue("remote_path") ?? ".";

        var files = More.GetValue("files", (object[])null);
        ArgumentNullException.ThrowIfNull(files);

        await LogHelper.LogDebugAsync(funcName, new { jobId, sshnet_settings_name, remote_path, files }, cancellationToken);

        using var sftp = await CreateSftpClient(sshnet_settings_name, cancellationToken);
        {
            sftp.Connect();

            foreach (var fileName in files)
            {
                var file = Convert.ToString(fileName);

                try
                {
                    if (cancellationToken.IsCancellationRequested) throw new Exception($"Сancelled Job: {file}.");

                    await sftp.UploadFileAsync(remote_path, file, cancellationToken);

                    await LogHelper.LogInformationAsync(funcName, new { jobId, errMsg = $"Uploaded: {file}." }, cancellationToken);
                }
                catch (Exception ex)
                {
                    await LogHelper.LogErrorAsync(funcName, new { jobId, errMsg = $"Failed: {file}. Error: {ex.Message}." }, cancellationToken);
                    throw;
                }
            }

            sftp.Disconnect();
        }

        await LogHelper.LogInformationAsync(funcName, new { jobId = JobId, result = "OK" }, cancellationToken);

        return true;
    }
}