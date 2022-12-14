// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Globalization;
using System.Text;
using CsvHelper;
using UkrGuru.Extensions;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions.CsvHelper;

public class ImportFileAction: BaseAction
{
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        const string funcName = "CsvHelper.ImportFile";

        var jobId = JobId;

        var file = More.GetValue("file");
        ArgumentNullException.ThrowIfNull(file);

        await WJbLogHelper.LogDebugAsync(funcName, new { jobId, file }, cancellationToken);

        WJbFile wjbFile = null;

        if (Guid.TryParse(file, out var guidFile))
            wjbFile = await WJbFileHelper.GetAsync(guidFile, cancellationToken);

        ArgumentNullException.ThrowIfNull(wjbFile);
        ArgumentNullException.ThrowIfNull(wjbFile.FileContent);

        await DbHelper.ExecProcAsync("WJbItems_Del_File", file, cancellationToken: cancellationToken);

        int count = 0;
        
        using (var stream = new MemoryStream(wjbFile.FileContent))
        using (var reader = new StreamReader(stream, Encoding.UTF8))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            int i = 0;
            while (await csv.ReadAsync())
            {
                var record = csv.GetRecord<dynamic>();

                await DbHelper.ExecProcAsync("WJbItems_Ins", new { FileId = file, ItemNo = i++, ItemMore = record }, cancellationToken: cancellationToken);
            }
            count = i;
        }

        await WJbLogHelper.LogInformationAsync(funcName, new { jobId = JobId, result = "OK", count }, cancellationToken);

        return true;
    }
}