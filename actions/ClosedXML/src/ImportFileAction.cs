using ClosedXML.Excel;
using UkrGuru.Extensions;
using UkrGuru.Extensions.Data;
using UkrGuru.Extensions.Logging;
using UkrGuru.SqlJson;

namespace UkrGuru.WebJobs.Actions.ClosedXML;

public class ImportFileAction : BaseAction
{
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        const string funcName = "ClosedXML.ImportFile";

        var jobId = JobId; int count = 0;

        var file = More.GetValue("file").ThrowIfBlank("file");

        await DbLogHelper.LogDebugAsync(funcName, new { jobId, file }, cancellationToken);

        count = await DbHelper.ExecAsync("WJbItems_Del_File", file, cancellationToken: cancellationToken);

        await DbLogHelper.LogInformationAsync(funcName, new { jobId, proc = "WJbItems_Del_File", file, result = "OK", count }, cancellationToken);

        DbFile dbFile = null;

        if (Guid.TryParse(file, out var guidFile))
            dbFile = await DbFileHelper.GetAsync(guidFile, cancellationToken);

        ArgumentNullException.ThrowIfNull(dbFile);
        ArgumentNullException.ThrowIfNull(dbFile.FileContent);

        count = 0;

        using (var stream = new MemoryStream(dbFile.FileContent))
        using (var wbook = new XLWorkbook(stream))
        {
            var ws1 = wbook.Worksheet(1);

            var head = new string[0];
            var dict = new Dictionary<string, object>();

            int r = 0;
            foreach (var row in ws1.Rows())
            {
                if (r == 0)
                {
                    head = row.GetHeadNames();
                }
                else
                {
                    dict.Fill(row, head);
                    await DbHelper.ExecAsync("WJbItems_Ins", new { FileId = file, ItemNo = r, ItemMore = dict }, cancellationToken: cancellationToken);
                }
                r++;
            }
        };

        await DbLogHelper.LogInformationAsync(funcName, new { jobId, proc = "WJbItems_Ins", file, result = "OK", count }, cancellationToken);

        return true;
    }
}