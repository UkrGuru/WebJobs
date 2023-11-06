// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.Extensions;
using UkrGuru.Extensions.Data;
using UkrGuru.Extensions.Logging;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions.SshNet;

public class GetFilesAction : SshNetAction
{

    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        const string funcName = "SshNet.GetFiles";

        var jobId = JobId;

        var sshnet_settings_name = More.GetValue("sshnet_settings_name").ThrowIfBlank("sshnet_settings_name");

        var remote_path = More.GetValue("remote_path") ?? ".";

        var proc_rule = More.GetValue("proc_rule"); 
        
        await DbLogHelper.LogDebugAsync(funcName, new { jobId, sshnet_settings_name, remote_path, proc_rule }, cancellationToken);

        using var sftp = await CreateSftpClient(sshnet_settings_name, cancellationToken);
        {
            sftp.Connect();

            var sftpFiles = await sftp.ListDirectoryAsync(remote_path, cancellationToken);

            foreach (var sftpFile in sftpFiles.Where(e => e.IsRegularFile).OrderBy(o => o.LastWriteTime))
            {
                if (cancellationToken.IsCancellationRequested) break;

                var remoteFullName = CombinateRemoteFullName(remote_path, sftpFile.Name);

                try
                {
                    var dbFile = new DbFile() { FileName = GetLocalFileName(sftpFile.Name) };
                    
                    dbFile.FileContent = await sftp.ReadAllBytesAsync(remoteFullName, cancellationToken);

                    var guidFile = await dbFile.SetAsync<string>(cancellationToken: cancellationToken);

                    await DbLogHelper.LogInformationAsync(funcName, new { jobId, result = $"Saved File: {guidFile}." });

                    if (!string.IsNullOrEmpty(proc_rule) && !string.IsNullOrEmpty(guidFile))
                    {
                        var proc_jobId = await DbHelper.ExecAsync<int?>("WJbQueue_Ins", new
                        {
                            Rule = proc_rule,
                            RulePriority = (byte)Priorities.ASAP,
                            RuleMore = new { file = guidFile }
                        }, cancellationToken: cancellationToken);

                        await DbLogHelper.LogInformationAsync(funcName, new { jobId, result = $"Created Proc Job: {proc_jobId}." });
                    }

                    await sftp.DeleteFileAsync(remoteFullName, cancellationToken);

                    await DbLogHelper.LogInformationAsync(funcName, new { jobId, errMsg = $"Downloaded: {remoteFullName}." }, cancellationToken);
                }
                catch (Exception ex)
                {
                    await DbLogHelper.LogErrorAsync(funcName, new { jobId, errMsg = $"Failed: {remoteFullName}. Error: {ex.Message}." }, cancellationToken);
                    throw;
                }
            }

            sftp.Disconnect();
        }

        await DbLogHelper.LogInformationAsync(funcName, new { jobId = JobId, result = "OK" }, cancellationToken);

        return true;
    }
}