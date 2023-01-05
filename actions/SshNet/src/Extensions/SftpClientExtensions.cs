// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Renci.SshNet;
using Renci.SshNet.Sftp;
using UkrGuru.Extensions;
using UkrGuru.Extensions.Data;

namespace UkrGuru.WebJobs.Actions.SshNet;

public static class SftpClientExtensions
{
    public static Task<IEnumerable<SftpFile>> ListDirectoryAsync(this SftpClient sftp, string path, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => { return sftp.ListDirectory(path); }, cancellationToken);
    }

    public static Task DeleteFileAsync(this SftpClient sftp, string path, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => { sftp.DeleteFile(path); }, cancellationToken);
    }

    public static Task<byte[]> ReadAllBytesAsync(this SftpClient sftp, string path, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => { return sftp.ReadAllBytes(path); }, cancellationToken);
    }

    public static Task WriteAllBytesAsync(this SftpClient sftp, string path, byte[] bytes, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => { sftp.WriteAllBytes(path, bytes); }, cancellationToken);
    }

    public static async Task<bool> UploadFileAsync(this SftpClient sftp, string path, string guid, CancellationToken cancellationToken = default)
    {
        if (Guid.TryParse(guid, out var guidFile))
        {
            var dbFile = await DbFileHelper.GetAsync(guidFile, cancellationToken);

            if (dbFile?.FileContent != null)
            {
                var remoteFullName = SshNetAction.CombinateRemoteFullName(path, dbFile.FileName);

                await sftp.WriteAllBytesAsync(remoteFullName, dbFile.FileContent, cancellationToken);

                return true;
            }
            else
                throw new Exception($"Unknown guidFile: {guidFile}");
        }
        else
            throw new Exception($"Invalid guidFile: {guidFile}");
    }
}